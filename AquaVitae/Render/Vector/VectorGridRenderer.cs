using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorGridRenderer(
    VectorElementRenderer elementRenderer,
    GridLayout gridLayout,
    PrintPoint width,
    Graphics graphics
)
{
    private readonly List<(int Row, float Height)> _rowHeights = [];
    
    public PrintPoint RenderGrid()
    {
        foreach (var contents in gridLayout.Cells)
        {                                                                                                                                                                                               
            switch (contents)
            {
                case GridCell cell:
                    RenderCell(cell);
                    break;
                case List<GridCell> cells:
                    foreach (var cell in cells)
                    {
                        RenderCell(cell);
                    }
                    break;
            }
        }
        
        if (_rowHeights.Count == 0) return PrintPoint.Zero;
        return _rowHeights[^1].Height;
    }
    
    private void RenderCell(GridCell cell)
    {
        var xWeight = cell.ColumnIndex == 0 ? 0 : gridLayout.ColumnWeights[cell.ColumnIndex - 1];
        var widthWeight = gridLayout.ColumnWeights[cell.CornerColumnIndex] - xWeight;

        var x = xWeight * width;
        var cellWidth = widthWeight * width;

        var cellGraphics = new Graphics();
        var cellHeight = elementRenderer.RenderElement(
            cell.Element,
            cellGraphics,
            cellWidth,
            cell.MaxHeight
        );
        
        if (cellHeight < cell.MinHeight)
        {
            cellHeight = cell.MinHeight;
        }

        if (cell.MaxHeight.HasValue && cellHeight > cell.MaxHeight.Value)
        {
            // TODO: Print out a warning that this cell was cropped!
            cellHeight = cell.MaxHeight.Value;
        }
            
        // We crop the cell to its rendered dimensions
        cellGraphics.Crop(new Point(0, 0), new Size(cellWidth.Points, cellHeight.Points));
        
        var y = GetY(cell.RowIndex);
        graphics.DrawGraphics(new Point(x.Points, y), cellGraphics);
        
        // We update the height of the row below us
        UpdateYHeight(cell.CornerRowIndex + 1, y + cellHeight.Points);
    }

    private float GetY(int rowIndex)
    {
        // First we find our y position
        if (rowIndex == 0) return 0;
        
        for (var i = _rowHeights.Count - 1; i >= 0; i--)
        {
            var (row, height) = _rowHeights[i];
            if (row > rowIndex) continue;
            return height;
        }

        return 0;
    }
    
    private void UpdateYHeight(int rowIndex, float height)
    {
        // If there are no entries, we must be the first height for this row
        if (_rowHeights.Count == 0)
        {
            _rowHeights.Add((rowIndex, height));
            return;
        }
        
        // If the entry is for an earlier row, we must be the first height for this row
        var (lastRow, lastHeight) = _rowHeights[^1];
        if (lastRow != rowIndex)
        {
            _rowHeights.Add((rowIndex, height));
            return;
        }
        
        // Elsewise, we compare to find the tallest element for this row
        _rowHeights[^1] = (rowIndex, MathF.Max(lastHeight, height));
    }
}