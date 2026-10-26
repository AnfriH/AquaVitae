using System.Diagnostics;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public class VectorPageRenderer
{
    private readonly VectorHyperlinkRenderer _hyperlinkRenderer = new();
    private readonly VectorElementRenderer _elementRenderer;
    private readonly Graphics _foregroundGraphics = new();
    private readonly Graphics _backgroundGraphics = new();
    
    private readonly VectorStyleRenderer _styleRenderer;
    private readonly PageLayout _pageLayout;
    private readonly Action<Page, VectorHyperlinkRenderer> _addPageCallback;
    
    // Margin properties
    private PrintPoint LeftMargin => _pageLayout.Margins.Left;
    private PrintPoint RightMargin => _pageLayout.Margins.Right;
    private PrintPoint TopMargin => _pageLayout.Margins.Top;
    private PrintPoint BottomMargin => _pageLayout.Margins.Bottom;
    
    // Page sizing properties
    private PrintPoint PageWidth => _pageLayout.PageSize.Width;
    private PrintPoint PageHeight => _pageLayout.PageSize.Height;
    private PrintPoint ContentsWidth { get; }
    private PrintPoint ContentsHeight { get; }
    
    public VectorPageRenderer(VectorStyleRenderer styleRenderer,
        PageLayout pageLayout,
        Action<Page, VectorHyperlinkRenderer> addPageCallback)
    {
        _styleRenderer = styleRenderer;
        _pageLayout = pageLayout;
        _addPageCallback = addPageCallback;
        _elementRenderer = new VectorElementRenderer(styleRenderer, _hyperlinkRenderer);
        
        ContentsWidth = PrintPoint.Zero.Max(PageWidth - LeftMargin - RightMargin);
        ContentsHeight = PrintPoint.Zero.Max(PageHeight - TopMargin - BottomMargin);
    }

    public void RenderPage()
    {
        var foregroundHeight = PrintPoint.Zero;
        var backgroundHeight = PrintPoint.Zero;
        
        // Render the layers onto the page canvas
        var background = _pageLayout.Background;
        if (background != null)
        {
            backgroundHeight = _elementRenderer.CanvasRenderer.RenderCanvas(
                background,
                _backgroundGraphics,
                new LayoutConstraints(0, 0, PageWidth, PageHeight)
            );
        }
        
        var grid = _pageLayout.Grid;
        if (grid != null)
        {
            foregroundHeight = new VectorGridRenderer(
                grid,
                _foregroundGraphics,
                new LayoutConstraints(0, 0, ContentsWidth, ContentsHeight),
                _elementRenderer,
                _pageLayout.OverflowBehaviour
            ).RenderGrid();
        }
        
        var foreground = _pageLayout.Foreground;
        if (foreground != null)
        {
            foregroundHeight = _elementRenderer.CanvasRenderer.RenderCanvas(
                foreground,
                _foregroundGraphics,
                new LayoutConstraints(0, 0, ContentsWidth, ContentsHeight)
            ).Max(foregroundHeight);
        }
        
        switch (_pageLayout.OverflowBehaviour)
        {
            case PageOverflowBehaviour.Scale:
            {
                RenderScaledPage(backgroundHeight, foregroundHeight);
                return;
            }
            case PageOverflowBehaviour.Truncate:
            {
                RenderTruncatedPage();
                return;
            }
            case PageOverflowBehaviour.Continuous:
            case PageOverflowBehaviour.PageFit:
                // Elsewise, we split the page up into multiple pieces
                //var pageCount = (int)MathF.Ceiling(actualPageHeight / height);
                RenderMultiplePages(foregroundHeight);
                return;
            default:
                throw new UnreachableException();
        }
    }

    private void RenderScaledPage(PrintPoint backgroundHeight, PrintPoint foregroundHeight)
    {
        var pageHeight = PageHeight.Max(backgroundHeight).Max(foregroundHeight + TopMargin + BottomMargin);
        
        var page = new Page(PageWidth.Points, pageHeight.Points)
        {
            Background = _pageLayout.PageColor.ToVectSharpColor()
        };
        
        page.Graphics.DrawGraphics(0, 0, _backgroundGraphics);
        page.Graphics.DrawGraphics(LeftMargin.Points, TopMargin.Points, _foregroundGraphics);
        
        AddPage(page);
    }

    private void RenderTruncatedPage()
    {
        _backgroundGraphics.Crop(new Rectangle(0, 0, PageWidth.Points, PageHeight.Points));
        _foregroundGraphics.Crop(new Rectangle(0, 0, ContentsWidth.Points, ContentsHeight.Points));
        
        var page = new Page(PageWidth.Points, PageHeight.Points)
        {
            Background = _pageLayout.PageColor.ToVectSharpColor()
        };
        
        page.Graphics.DrawGraphics(0, 0, _backgroundGraphics);
        page.Graphics.DrawGraphics(LeftMargin.Points, TopMargin.Points, _foregroundGraphics);
        
        AddPage(page);
    }

    private void RenderMultiplePages(PrintPoint foregroundHeight)
    {
        var pagesRequired = (int)MathF.Ceiling(foregroundHeight / ContentsHeight);
        var background = _pageLayout.PageColor.ToVectSharpColor();

        for (var i = 0; i < pagesRequired; i++)
        {
            var page = new Page(PageWidth.Points, PageHeight.Points) { Background = background };
            
            // Background graphics are repeated per page
            page.Graphics.DrawGraphics(0, 0, _backgroundGraphics);

            // Foreground graphics are spread across the pages
            var pageForegroundGraphics = new Graphics();
            pageForegroundGraphics.DrawGraphics(
                0, 
                -i * (ContentsHeight.Points + VectorGridRenderer.PageBuffer.Points),
                _foregroundGraphics
            );
            pageForegroundGraphics.Crop(new Rectangle(0, 0, ContentsWidth.Points, ContentsHeight.Points));
            
            page.Graphics.DrawGraphics(LeftMargin.Points, TopMargin.Points, pageForegroundGraphics);
            
            AddPage(page);
        }
    }

    private void AddPage(Page page)
    {
        _addPageCallback(page, _hyperlinkRenderer);
    }
}