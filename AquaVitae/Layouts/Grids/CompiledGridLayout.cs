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
        foreach (var element in Elements)
        {
            element.Element.CollectParagraphs(callback);
        }
    }
}