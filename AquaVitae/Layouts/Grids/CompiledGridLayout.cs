using System.Collections.Immutable;
using AquaVitae.Layouts.Abstractions;

namespace AquaVitae.Layouts.Grids;

public sealed class CompiledGridLayout(
    List<CompiledGridElement> elements,
    List<ImmutableArray<int>> childIndices,
    List<int> parentCounts,
    ImmutableArray<int> rootIndices
) : ElementLayoutBase
{
    public IReadOnlyList<CompiledGridElement> Elements => elements;
    public IReadOnlyList<ImmutableArray<int>> ChildIndices => childIndices;
    public IReadOnlyList<int> ParentCounts => parentCounts;
    public ImmutableArray<int> RootIndices => rootIndices;
    
    public override void CollectParagraphs(Action<ParagraphLayoutBase> callback)
    {
        var frontier = new Stack<int>();
        var remainingParents = ParentCounts.ToArray();
        
        for (var i = RootIndices.Length - 1; i >= 0; i--)
        {
            frontier.Push(RootIndices[i]);
        }

        while (frontier.Count > 0)
        {
            var index = frontier.Pop();
            var element = Elements[index];
            
            element.Element.CollectParagraphs(callback);

            if (element.ChildrenIndex == -1 || --remainingParents[element.ChildrenIndex] > 0) continue;
            
            var childrenIndices = ChildIndices[element.ChildrenIndex];
            for (var i = childrenIndices.Length - 1; i >= 0; i--)
            {
                frontier.Push(childrenIndices[i]);
            }
        }
    }
}