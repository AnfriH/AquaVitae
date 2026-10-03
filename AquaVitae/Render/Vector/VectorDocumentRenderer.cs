using System.Xml;
using AquaVitae.Layouts;
using VectSharp;
using VectSharp.PDF;
using VectSharp.SVG;

namespace AquaVitae.Render.Vector;

public sealed class VectorDocumentRenderer
{
    private int _uriIndex;
    private readonly Dictionary<Uri, string> _uriRegistry = new();
    private readonly Dictionary<string, string> _linkDestinations = new();

    public XmlDocument[] RenderAsSvgPages(DocumentLayout documentLayout)
    {
        var pages = RenderPages(documentLayout);
        
        var pageSvgs = new XmlDocument[pages.Count];
        for (var i = 0; i < pageSvgs.Length; i++)
        {
            pageSvgs[i] = pages[i].SaveAsSVG(
                SVGContextInterpreter.TextOptions.ConvertIntoPathsUsingGlyphs,
                _linkDestinations
            );
        }
        
        return pageSvgs;
    }

    public PDFDocument RenderAsPdf(DocumentLayout documentLayout)
    {
        var pages = RenderPages(documentLayout);
        var document = new Document { Pages = pages };
        
        return document.CreatePDFDocument(linkDestinations: _linkDestinations);
    }
    
    private List<Page> RenderPages(DocumentLayout documentLayout)
    {
        var styles = new VectorStyleRenderer(documentLayout.Styles);
        
        var pageRenderer = new VectorPageRenderer(styles);
        var pageLayouts = documentLayout.Pages;
        
        var pages = new List<Page>(pageLayouts.Count);
        foreach (var pageLayout in pageLayouts)
        {
            pageRenderer.RenderPage(pageLayout, pages);
        }
        
        return pages;
    }

    public string AddUri(Uri uri)
    {
        if (_uriRegistry.TryGetValue(uri, out var id)) return id;
        id = $"link{_uriIndex++}";
        
        _uriRegistry.Add(uri, id);
        _linkDestinations.Add(id, uri.ToString());
        
        return id;
    }
}