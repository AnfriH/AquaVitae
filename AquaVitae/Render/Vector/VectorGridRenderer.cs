using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorGridRenderer
{
    private readonly float _widthPoints;
    private readonly float _heightPoints;
    
    private readonly List<(int Row, float Height)> _rowHeights = [];
    private readonly VectorElementRenderer _elementRenderer;
    private readonly GridLayout _gridLayout;
    private readonly Graphics _graphics;
    private readonly PageOverflowBehaviour _overflowBehaviour;
    
    public VectorGridRenderer(
        VectorElementRenderer elementRenderer,
        GridLayout gridLayout,
        PrintPoint width,
        PrintPoint height,
        Graphics graphics,
        PageOverflowBehaviour overflowBehaviour = PageOverflowBehaviour.Truncate
    )
    {
        _elementRenderer = elementRenderer;
        _gridLayout = gridLayout;
        _graphics = graphics;
        _overflowBehaviour = overflowBehaviour;
        _widthPoints = width.Points;
        _heightPoints = height.Points;
    }

    public VectorGridRenderer(
        VectorElementRenderer elementRenderer,
        GridLayout gridLayout,
        PrintPoint width,
        Graphics graphics
    )
    {
        _elementRenderer = elementRenderer;
        _gridLayout = gridLayout;
        _graphics = graphics;
        _overflowBehaviour = PageOverflowBehaviour.Truncate;
        _widthPoints = width.Points;
        
        // I set this to NaN as it should not be used if instantiated with this constructor
        _heightPoints = float.NaN;
    }

    public PrintPoint RenderGrid()
    {
        foreach (var contents in _gridLayout.Cells)
        {
            var rendered = true;
            switch (contents)
            {
                case GridCell cell:
                    rendered &= RenderCell(cell);
                    break;
                case List<GridCell> cells:
                    foreach (var cell in cells)
                    {
                        rendered &= RenderCell(cell);
                    }
                    break;
            }
            if (!rendered) break;
        }
        
        if (_rowHeights.Count == 0) return PrintPoint.Zero;
        return _rowHeights[^1].Height;
    }
    
    // True means that this cell was rendered successfully; false means that it was not (usually because it was truncated).
    private bool RenderCell(GridCell cell)
    {
        var xWeight = cell.ColumnIndex == 0 ? 0 : _gridLayout.ColumnWeights[cell.ColumnIndex - 1];
        var widthWeight = _gridLayout.ColumnWeights[cell.CornerColumnIndex] - xWeight;

        var cellWidth = widthWeight * _widthPoints;
        var x = xWeight * _widthPoints;
        var y = GetY(cell.RowIndex);

        if (_overflowBehaviour == PageOverflowBehaviour.Truncate && _heightPoints < y)
        {
            return false;
        }

        var cellGraphics = new Graphics();
        var cellHeight = _elementRenderer.RenderElement(
            cell.Element,
            cellGraphics,
            cellWidth,
            cell.MaxHeight
        ).Points;

        var minHeight = cell.MinHeight.Points;
        if (cellHeight < minHeight)
        {
            cellHeight = minHeight;
        }
        
        if (cell.MaxHeight.HasValue && cellHeight > cell.MaxHeight.Value.Points)
        {
            // TODO: Print out a warning that this cell was cropped!
            cellHeight = cell.MaxHeight.Value.Points;
        }
        
        if (_overflowBehaviour == PageOverflowBehaviour.PageFit)
        {
            // If the cell is between pages, we shift it down to the start of the next page.
            // This is the only place where we use _heightPoints.
            var topPageIndex = MathF.Floor(y / _heightPoints);
            var bottomPageIndex = MathF.Floor((y + cellHeight) / _heightPoints);

            var pageDifference = (int)MathF.Abs(bottomPageIndex - topPageIndex);
            if (pageDifference == 1) y = bottomPageIndex * _heightPoints;
        }
            
        // We crop the cell to its rendered dimensions
        cellGraphics.Crop(new Rectangle(0, 0, cellWidth, cellHeight));
        _graphics.DrawGraphics(x, y, cellGraphics);
        
        // We update the height of the row below us
        UpdateYHeight(cell.CornerRowIndex + 1, y + cellHeight);

        return true;
    }

    private float GetY(int rowIndex)
    {
        // First we find our y position
        if (rowIndex == 0) return 0;
        
        for (var i = _rowHeights.Count - 1; i >= 0; i--)
        {
            var (row, rowHeight) = _rowHeights[i];
            if (row > rowIndex) continue;
            return rowHeight;
        }

        return 0;
    }
    
    private void UpdateYHeight(int rowIndex, float rowHeight)
    {
        // If there are no entries, we must be the first height for this row
        if (_rowHeights.Count == 0)
        {
            _rowHeights.Add((rowIndex, rowHeight));
            return;
        }
        
        // If the entry is for an earlier row, we must be the first height for this row
        var (lastRow, lastHeight) = _rowHeights[^1];
        if (lastRow != rowIndex)
        {
            _rowHeights.Add((rowIndex, rowHeight));
            return;
        }
        
        // Elsewise, we compare to find the tallest element for this row
        _rowHeights[^1] = (rowIndex, MathF.Max(lastHeight, rowHeight));
    }
}