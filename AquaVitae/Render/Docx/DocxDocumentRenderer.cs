using AquaVitae.Layouts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public sealed class DocxDocumentRenderer
{
    public void RenderDocument(DocumentLayout document, Stream outputStream)
    {
        using var wordDocument = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document);
        
        var mainPart = wordDocument.AddMainDocumentPart();
        
        RenderBody(document, mainPart);
    }

    private static void RenderBody(DocumentLayout document, MainDocumentPart mainPart)
    {
        var body = new Body();
        var pages = document.Pages;
        
        var numberingRenderer = new DocxNumberingRenderer(document, mainPart);
        var hyperlinkRenderer = new DocxHyperlinkRenderer(mainPart);
        var stylesRenderer = new DocxStylesRenderer(document, mainPart);
        
        var pageRenderer = new DocxPageRenderer(numberingRenderer, hyperlinkRenderer, stylesRenderer);
        
        for (var i = 0; i < pages.Count; i++)
        {
            var page = pages[i];
            var finalPage = i == pages.Count - 1;

            pageRenderer.RenderPage(page, body, finalPage);
        }
        
        mainPart.Document = new Document { Body = body };
    }
}