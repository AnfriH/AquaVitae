using System.Diagnostics;
using AquaVitae.Layouts;
using VectSharp;

namespace AquaVitae.Render.Vector;

public class VectorPageRenderer(
    VectorStyleRenderer styleRenderer
)
{
    public void RenderPage(PageLayout pageLayout, Action<Page, VectorHyperlinkRenderer> addPageCallback)
    {
        var hyperlinkRenderer = new VectorHyperlinkRenderer();
        var elementRenderer = new VectorElementRenderer(styleRenderer, hyperlinkRenderer);
        var (width, height) = pageLayout.PageSize;
        
        var backgroundGraphics = new Graphics();
        var graphics = new Graphics();

        var actualPageHeight = 0f;
        
        // Render the layers onto the page canvas
        var background = pageLayout.Background;
        if (background != null)
        {
            actualPageHeight = MathF.Max(
                actualPageHeight,
                elementRenderer.CanvasRenderer.RenderCanvas(background, backgroundGraphics, 0, 0).Points
            );
        }
        
        var grid = pageLayout.Grid;
        if (grid != null)
        {
            actualPageHeight = MathF.Max(
                actualPageHeight,
                new VectorGridRenderer(
                    elementRenderer, 
                    grid, 
                    width, 
                    height, 
                    graphics, 
                    pageLayout.OverflowBehaviour
                ).RenderGrid().Points
            );
        }
        
        var foreground = pageLayout.Foreground;
        if (foreground != null)
        {
            actualPageHeight = MathF.Max(
                actualPageHeight,
                elementRenderer.CanvasRenderer.RenderCanvas(foreground, graphics, 0, 0).Points
            );
        }
        
        switch (pageLayout.OverflowBehaviour)
        {
            case PageOverflowBehaviour.Scale:
            {
                RenderSinglePage(MathF.Max(actualPageHeight, height.Points));
                return;
            }
            case PageOverflowBehaviour.Truncate:
            {
                RenderSinglePage(height.Points);
                return;
            }
            case PageOverflowBehaviour.Continuous:
            case PageOverflowBehaviour.PageFit:
                // Elsewise, we split the page up into multiple pieces
                var pageCount = MathF.Ceiling(actualPageHeight / height.Points);
                for (var i = 0; i < pageCount; i++)
                {
                    var page = new Page(width.Points, height.Points)
                    {
                        Background = pageLayout.PageColor.ToVectSharpColor()
                    };
                    var pageGraphics = page.Graphics;
            
                    // We repeat the background graphics for every page
                    pageGraphics.DrawGraphics(0, 0, backgroundGraphics);
                    pageGraphics.DrawGraphics(0, -height.Points * i, graphics);
                    pageGraphics.Crop(new Rectangle(0, 0, width.Points, height.Points));
            
                    addPageCallback(page, hyperlinkRenderer);
                }
                return;
            default:
                throw new UnreachableException();
        }
        
        void RenderSinglePage(float pageHeight)
        {
            var page = new Page(width.Points, pageHeight)
            {
                Background = pageLayout.PageColor.ToVectSharpColor(),
                Graphics = backgroundGraphics
            };
            page.Graphics.DrawGraphics(0, 0, graphics);
            addPageCallback(page, hyperlinkRenderer);
        }
    }
}