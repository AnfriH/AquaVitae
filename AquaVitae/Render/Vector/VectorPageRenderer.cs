using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public class VectorPageRenderer()
{
    public Page RenderPage(PageLayout pageLayout)
    {
        var (width, height) = pageLayout.PageSize;
        var page = new Page(width.Points, height.Points);
        return page;
    }
}