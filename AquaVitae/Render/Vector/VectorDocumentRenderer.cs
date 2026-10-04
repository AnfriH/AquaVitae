using System.Xml.Linq;
using AquaVitae.Layouts;
using VectSharp;
using VectSharp.PDF;
using VectSharp.SVG;

namespace AquaVitae.Render.Vector;

public sealed class VectorDocumentRenderer
{
    private static readonly Dictionary<string, string> Empty = new();
    
    public SvgDocument RenderAsSvg(DocumentLayout documentLayout)
    {
        var pageSvgs = new List<SvgPage>();
        RenderPages(documentLayout, AddPage);
        return new SvgDocument(pageSvgs);

        void AddPage(Page page, VectorHyperlinkRenderer hyperlinkRenderer)
        {
            var svg = page.SaveAsSVG(
                SVGContextInterpreter.TextOptions.ConvertIntoPathsUsingGlyphs,
                hyperlinkRenderer.Hyperlinks
            );
            
            pageSvgs.Add(new SvgPage(
                (float)page.Width, 
                (float)page.Height, 
                svg,
                hyperlinkRenderer.LinkPositions
            ));
        }
    }

    public PDFDocument RenderAsPdf(DocumentLayout documentLayout)
    {
        var hyperlinks = new Dictionary<string, string>();
        var pages = new List<Page>();
        
        RenderPages(documentLayout, AddPage);
        var document = new Document { Pages = pages };
        
        return document.CreatePDFDocument(linkDestinations: hyperlinks);

        void AddPage(Page page, VectorHyperlinkRenderer hyperlinkRenderer)
        {
            pages.Add(page);
            if (hyperlinkRenderer.Hyperlinks == null) return;

            foreach (var (key, value) in hyperlinkRenderer.Hyperlinks)
            {
                hyperlinks.Add(key, value);
            }
        }
    }
    
    private void RenderPages(DocumentLayout documentLayout, Action<Page, VectorHyperlinkRenderer> addPageCallback)
    {
        var styles = new VectorStyleRenderer(documentLayout.Styles);
        
        var pageRenderer = new VectorPageRenderer(styles);
        var pageLayouts = documentLayout.Pages;
        
        foreach (var pageLayout in pageLayouts)
        {
            pageRenderer.RenderPage(pageLayout, addPageCallback);
        }
    }
}