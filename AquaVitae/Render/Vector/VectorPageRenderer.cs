using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public class VectorPageRenderer(VectorStyleRenderer styleRenderer)
{
    private readonly VectorElementRenderer _elementRenderer = new(styleRenderer);
    
    public Page RenderPage(PageLayout pageLayout)
    {
        var (width, height) = pageLayout.PageSize;
        var page = new Page(width.Points, height.Points);
        var graphics = page.Graphics;
        
        if (pageLayout.OverflowBehaviour != PageOverflowBehaviour.Truncate)
        {
            throw new NotSupportedException("Only truncation is currently supported");
        }
        
        _elementRenderer.RenderElement(pageLayout.Background, graphics, width, height);
        _elementRenderer.RenderElement(pageLayout.Grid, graphics, width, height);
        _elementRenderer.RenderElement(pageLayout.Foreground, graphics, width, height);
        
        return page;
    }
}