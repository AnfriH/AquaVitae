using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

/// <summary>
/// Represents a grid layout container. 
/// </summary>
public sealed class GridLayout : ElementLayoutBase
{
    private readonly float[] _columnWeights;
    public IReadOnlyList<float> ColumnWeights => _columnWeights;
    private readonly SortedDictionary<(int CornerRow, int CornerColumn), GridCellContents> _cells = [];
    public ICollection<GridCellContents> Cells => _cells.Values;
    
    public int Rows { get; private set; }
    public int Columns => _columnWeights.Length;
    
    public GridLayout(params float[] columnWeights)
    {
        var totalWeight = columnWeights.Sum();
        var accumulator = 0f;
        
        var accumulatedWeights = new float[columnWeights.Length];
        for (var i = 0; i < columnWeights.Length; i++)
        {
            accumulator += columnWeights[i] / totalWeight;
            accumulatedWeights[i] = accumulator;
        }

        _columnWeights = accumulatedWeights;
    }
    
    public GridLayout AddCell(GridCell cell)
    {
        var cornerRowIndex = cell.CornerRowIndex;
        var cornerColumnIndex = cell.CornerColumnIndex;
        
        if (cornerColumnIndex > Columns)
        {
            throw new ArgumentException(
                $"Cell column span exceeds number of columns: " +
                $"{cell.ColumnIndex} + span {cell.ColumnSpan} > {Columns}"
            );
        }

        Rows = Math.Max(Rows, cornerRowIndex);

        var contents = _cells.GetValueOrDefault((cornerRowIndex, cornerColumnIndex));
        
        switch (contents)
        {
            case GridCell existingCell:
                _cells[(cornerRowIndex, cornerColumnIndex)] = new List<GridCell>(2) { existingCell, cell };
                break;
            case List<GridCell> existingCells:
                existingCells.Add(cell);
                break;
            default:
                _cells[(cornerRowIndex, cornerColumnIndex)] = cell;
                break;
        }
        
        return this;
    }

    public override void CollectParagraphs(Action<ParagraphLayoutBase> callback)
    {
        foreach (var contents in _cells.Values)
        {
            switch (contents)
            {
                case GridCell cell:
                    cell.Element.CollectParagraphs(callback);
                    break;
                case List<GridCell> cells:
                    foreach (var cell in cells)
                    {
                        cell.Element.CollectParagraphs(callback);
                    }
                    break;
            }
        }
    }

    public union GridCellContents(GridCell, List<GridCell>);
}

public sealed class GridCell(
    ElementLayoutBase element,
    int rowIndex,
    int columnIndex
)
{
    public int RowIndex => rowIndex;
    public int ColumnIndex => columnIndex;
    public int ColumnSpan { get; init; } = 1;
    public int RowSpan { get; init; } = 1;
    
    public PrintPoint MinHeight { get; init; }
    public PrintPoint? MaxHeight { get; init; }
    public ElementLayoutBase Element => element;
    
    public int CornerRowIndex => RowIndex + RowSpan - 1;
    public int CornerColumnIndex => ColumnIndex + ColumnSpan - 1;
}