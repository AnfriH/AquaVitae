using System.Collections.Immutable;

namespace AquaVitae.Layouts.Grids;

public abstract class GridBuilderBase
{
    /// <summary>
    /// The cumulative fraction of all columns up to and including the current column.
    /// For example, if the column ratios were [2, 3, 4, 1], the fractions would be [0.2, 0.5, 0.9].
    /// The final cumulative fraction is always 1, so we omit it.
    /// Use <see cref="GetColumnFraction"/> to retrieve the non-cumulative width of a column.
    /// </summary>
    protected readonly float[] ColumnFractions;
    
    protected GridBuilderBase(float[] columnRatios)
    {
        if (columnRatios.Length == 0) throw new ArgumentException("At least one column ratio must be provided.");
        
        var totalRatio = columnRatios.Sum();
        ColumnFractions = new float[columnRatios.Length - 1];
        
        var cumulativeRatio = 0f;
        for (var i = 0; i < ColumnFractions.Length; i++)
        {
            cumulativeRatio += columnRatios[i];
            ColumnFractions[i] = cumulativeRatio / totalRatio;
        }
    }
    
    protected (float XOffset, float Width) GetColumnBounds(int column, int columnSpan)
    {
        // left is the width of all columns up to but not including the left column
        float left;
        if (column <= 0) { left = 0f; }
        else if (column > ColumnFractions.Length) { left = 1f; }
        else { left = ColumnFractions[column - 1]; }
        
        var rightColumn = column + columnSpan - 1;
        
        // right is the width of all columns up to and including the right column
        float right;
        if (rightColumn < 0) { right = 0f; }
        else if (rightColumn >= ColumnFractions.Length) { right = 1f; }
        else { right = ColumnFractions[rightColumn]; }

        return (left, right - left);
    }
    
    protected internal abstract ImmutableArray<int> Compile(
        float xOffset,
        float width,
        int outerChildrenIndex,
        MutableCompiledGridLayout layout
    );

    protected internal class MutableCompiledGridLayout
    {
        private List<CompiledGridElement> Elements { get; } = [];
        private List<ImmutableArray<int>> ChildrenIndices { get; } = [];
        private List<int> ParentCounts { get; } = [];

        public int AddElement(CompiledGridElement element)
        {
            var index = Elements.Count;
            Elements.Add(element);
            if (element.ChildrenIndex != -1) ParentCounts[element.ChildrenIndex]++;
            return index;
        }
        
        public CompiledGridLayout ToLayout(ImmutableArray<int> rootIndices)
        {
            return new CompiledGridLayout(Elements, ChildrenIndices, ParentCounts, rootIndices);
        }

        public ImmutableArray<int> GetIndices(int index)
        {
            return index == -1 
                ? ImmutableArray<int>.Empty 
                : ChildrenIndices[index];
        }

        public int AddIndices(ImmutableArray<int> indices)
        {
            var index = ChildrenIndices.Count;
            ChildrenIndices.Add(indices);
            ParentCounts.Add(0);
            return index;
        }
    }
}