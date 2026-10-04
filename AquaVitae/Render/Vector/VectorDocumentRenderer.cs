using System.Xml;
using AquaVitae.Layouts;
using VectSharp;
using VectSharp.PDF;
using VectSharp.SVG;

namespace AquaVitae.Render.Vector;

public sealed class VectorDocumentRenderer
{
    private readonly VectorHyperlinkRenderer _hyperlinkRenderer = new();

    public SvgDocument RenderAsSvg(DocumentLayout documentLayout)
    {
        var pages = RenderPages(documentLayout);
        
        var pageSvgs = new SvgPage[pages.Count];
        for (var i = 0; i < pageSvgs.Length; i++)
        {
            var page = pages[i];
            var svg = page.SaveAsSVG(
                SVGContextInterpreter.TextOptions.ConvertIntoPathsUsingGlyphs,
                _hyperlinkRenderer.Hyperlinks
            );
            
            pageSvgs[i] = new SvgPage((float)page.Width, (float)page.Height, svg);
        }
        
        return new SvgDocument(pageSvgs);
    }

    public PDFDocument RenderAsPdf(DocumentLayout documentLayout)
    {
        var pages = RenderPages(documentLayout);
        var document = new Document { Pages = pages };
        
        return document.CreatePDFDocument(linkDestinations: _hyperlinkRenderer.Hyperlinks);
    }
    
    private List<Page> RenderPages(DocumentLayout documentLayout)
    {
        var styles = new VectorStyleRenderer(documentLayout.Styles);
        
        var pageRenderer = new VectorPageRenderer(styles, _hyperlinkRenderer);
        var pageLayouts = documentLayout.Pages;
        
        var pages = new List<Page>();
        foreach (var pageLayout in pageLayouts)
        {
            pageRenderer.RenderPage(pageLayout, pages);
        }
        
        return pages;
    }
}