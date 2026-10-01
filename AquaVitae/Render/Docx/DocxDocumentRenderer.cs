using System.Xml;
using AquaVitae.Layouts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public sealed class DocxDocumentRenderer(DocxRendererSettings settings)
{
    public void RenderDocument(DocumentLayout document, Stream outputStream, XmlDocument[]? pageSvgs = null)
    {
        using var wordDocument = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document);
        
        var mainPart = wordDocument.AddMainDocumentPart();
        
        RenderBody(document, mainPart, pageSvgs ?? []);
    }

    private void RenderBody(DocumentLayout document, MainDocumentPart mainPart, XmlDocument[] pageSvgs)
    {
        var body = new Body();
        var pages = document.Pages;
        
        var numberingRenderer = new DocxNumberingRenderer(document, mainPart);
        var hyperlinkRenderer = new DocxHyperlinkRenderer(mainPart);
        var stylesRenderer = new DocxStylesRenderer(document, mainPart, settings);
        var svgRenderer = new DocxSvgRenderer(mainPart);
        
        var pageRenderer = new DocxPageRenderer(numberingRenderer, hyperlinkRenderer, stylesRenderer, svgRenderer);
        
        for (var i = 0; i < pages.Count; i++)
        {
            var page = pages[i];
            var pageSvg = pageSvgs.Length > i ? pageSvgs[i] : null;
            var finalPage = i == pages.Count - 1;

            pageRenderer.RenderPage(page, body, finalPage, pageSvg);
        }
        
        mainPart.Document = new Document { Body = body };
    }
}