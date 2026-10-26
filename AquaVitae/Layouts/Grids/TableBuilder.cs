using System.Collections.Immutable;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Grids;

public sealed class TableBuilder(params float[] columnRatios) : GridBuilderBase(columnRatios)
{
    private readonly List<CellBase> _cells = [];

    public TableBuilder AddCell(CellBase cell)
    {
        _cells.Add(cell);
        return this;
    }
    
    public CompiledGridLayout Compile()
    {
        var layout = new MutableCompiledGridLayout();
        var rootIndices = Compile(0, 1, -1, layout);
        return layout.ToLayout(rootIndices);
    }

    protected internal override ImmutableArray<int> Compile(
        float xOffset,
        float width,
        int outerChildrenIndex,
        MutableCompiledGridLayout layout
    )
    {
        // If there are no cells
        if (_cells.Count == 0) return layout.GetIndices(outerChildrenIndex);
        
        _cells.Sort();
        
        var childIndicesLookup = new List<(int Row, int ChildrenIndex)>();
        var rowBuilder = ImmutableArray.CreateBuilder<int>();
        
        var currentRow = _cells[0].Row;
        foreach (var cell in _cells)
        {
            if (cell.Row != currentRow)
            {
                var indices = rowBuilder.ToImmutable();
                rowBuilder.Clear();
                
                var rowIndex = layout.AddIndices(indices);
                childIndicesLookup.Add((currentRow, rowIndex));
                
                currentRow = cell.Row;
            }
            
            var (innerXOffset, innerWidth) = GetColumnBounds(cell.Column, cell.ColumnSpan);
            innerXOffset = innerXOffset * width + xOffset;
            innerWidth *= width;

            var childrenIndex = GetChildrenIndex(cell.Row + cell.RowSpan);

            if (cell is ElementCell elementCell)
            {
                var compiledElement = new CompiledGridElement(
                    innerXOffset,
                    innerWidth,
                    elementCell.MinimumHeight,
                    elementCell.MaximumHeight,
                    childrenIndex,
                    elementCell.Element
                );
                var elementIndex = layout.AddElement(compiledElement);
                rowBuilder.Add(elementIndex);
            }
            else if (cell is SubgridCell subGridCell)
            {
                // add the sub-builder as if it was inlined within this row
                var childIndices = subGridCell.SubBuilder.Compile(
                    innerXOffset,
                    innerWidth,
                    childrenIndex,
                    layout
                );
                rowBuilder.AddRange(childIndices);
            }
        }

        return rowBuilder.DrainToImmutable();

        int GetChildrenIndex(int lookupRow)
        {
            for (var i = childIndicesLookup.Count - 1; i >= 0; i--)
            {
                var (foundRow, foundIndex) = childIndicesLookup[i];
                if (foundRow < lookupRow) continue;
                return foundIndex;
            }
        
            return outerChildrenIndex;
        }
    }
}

public closed class CellBase(int row, int column) : IComparable<CellBase>
{
    public int Row => row;
    public int Column => column;
    public int RowSpan { get; init; } = 1;
    public int ColumnSpan { get; init; } = 1;
        
    public int CompareTo(CellBase? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return -1;
        
        // Reversed order, we get the lowest row first
        return other.Row.CompareTo(Row);
    }
}

public sealed class ElementCell(ElementLayoutBase element, int row, int column) : CellBase(row, column)
{
    public PrintPoint MinimumHeight { get; init; } = PrintPoint.Zero;
    public PrintPoint MaximumHeight { get; init; } = PrintPoint.PositiveInfinity;
    public ElementLayoutBase Element => element;
}

public sealed class SubgridCell(GridBuilderBase subBuilder, int row, int column) : CellBase(row, column)
{
    public GridBuilderBase SubBuilder => subBuilder;
}