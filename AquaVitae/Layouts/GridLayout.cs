using AquaVitae.Common;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class GridLayout(params PrintPoint[] columnWidths) : LayoutBase
{
    public IReadOnlyList<PrintPoint> ColumnWidths => columnWidths;
    private OptionalList<GridCell> _cells = [];
    public IReadOnlyList<GridCell> Cells => _cells.AsReadOnly();

    public GridLayout AddCell(GridCell cell)
    {
        if (cell.ColumnIndex + cell.ColumnSpan - 1 > columnWidths.Length)
        {
            throw new ArgumentException(
                $"Cell column span exceeds column width: " +
                $"{cell.ColumnIndex} + span {cell.ColumnSpan} > {columnWidths.Length}"
            );
        }

        _cells.Add(cell);
        return this;
    }
}

public sealed class GridCell(
    LayoutBase element,
    int rowIndex,
    int columnIndex
) : IComparable<GridCell>
{
    public int RowIndex { get; } = rowIndex;
    public int ColumnIndex { get; } = columnIndex;
    public int ColumnSpan { get; init; } = 1;
    public int RowSpan { get; init; } = 1;
    
    public PrintPoint MinHeight { get; }
    public PrintPoint MaxHeight { get; }
    public LayoutBase Element { get; } = element;
    
    public int CompareTo(GridCell? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;

        var bottomRowIndex = RowIndex + RowSpan - 1;
        var otherBottomRowIndex = other.RowIndex + other.RowSpan - 1;
        
        // First sort by cell vertical position
        if (bottomRowIndex < otherBottomRowIndex) return -1;
        if (bottomRowIndex > otherBottomRowIndex) return 1;
        
        var rightColumnIndex = ColumnIndex + ColumnSpan - 1;
        var otherRightColumnIndex = other.ColumnIndex + other.ColumnSpan - 1;
        
        // Then sort by cell horizontal position
        if (rightColumnIndex < otherRightColumnIndex) return -1;
        if (rightColumnIndex > otherRightColumnIndex) return 1;

        return 0;
    }
}