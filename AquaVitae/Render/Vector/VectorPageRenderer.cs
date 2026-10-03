using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public class VectorPageRenderer(VectorStyleRenderer styleRenderer)
{
    private readonly VectorElementRenderer _elementRenderer = new(styleRenderer);
    
    public void RenderPage(PageLayout pageLayout, List<Page> pages)
    {
        var (width, height) = pageLayout.PageSize;
        
        var backgroundGraphics = new Graphics();
        var graphics = new Graphics();

        var actualPageHeight = 0f;
        
        var background = pageLayout.Background;
        if (background != null)
        {
            actualPageHeight = MathF.Max(
                actualPageHeight,
                _elementRenderer.CanvasRenderer.RenderCanvas(background, backgroundGraphics).Points
            );
        }
        var grid = pageLayout.Grid;
        if (grid != null)
        {
            actualPageHeight = MathF.Max(
                actualPageHeight,
                _elementRenderer.RenderElement(grid, graphics, width, height).Points
            );
        }
        var foreground = pageLayout.Foreground;
        if (foreground != null)
        {
            actualPageHeight = MathF.Max(
                actualPageHeight,
                _elementRenderer.CanvasRenderer.RenderCanvas(foreground, graphics).Points
            );
        }
        
        if (pageLayout.OverflowBehaviour == PageOverflowBehaviour.Scale)
        {
            // In the case of a single page, we just scale the page height to fit everything
            var pageHeight = MathF.Max(actualPageHeight, height.Points);
            var page = new Page(width.Points, pageHeight)
            {
                Background = pageLayout.PageColor.ToVectSharpColor()
            };
            var pageGraphics = page.Graphics;
            pageGraphics.DrawGraphics(0, 0, backgroundGraphics);
            pageGraphics.DrawGraphics(0, 0, graphics);
            pages.Add(page);
            return;
        }
        
        // Elsewise, we split the page up into multiple pieces
        var pageCount = MathF.Ceiling(height.Points / actualPageHeight);
        for (var i = 0; i < pageCount; i++)
        {
            var page = new Page(width.Points, actualPageHeight)
            {
                Background = pageLayout.PageColor.ToVectSharpColor()
            };
            var pageGraphics = page.Graphics;
            
            // Each page gets a copy of the background graphics, and a slice of the grid and foreground graphics
            pageGraphics.DrawGraphics(0, 0, backgroundGraphics);
            pageGraphics.DrawGraphics(0, -height.Points * i, graphics);
            pageGraphics.Crop(new Rectangle(0, 0, width.Points, height.Points));
            
            pages.Add(page);
        }
    }
}