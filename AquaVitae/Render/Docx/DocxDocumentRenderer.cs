using AquaVitae.Layouts;
using AquaVitae.Render.Abstractions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public sealed class DocxDocumentRenderer : IDocumentRenderer
{
    private readonly DocxStylesRenderer _stylesRenderer = new();
    
    public Task RenderDocumentAsync(DocumentLayout document, Stream outputStream)
    {
        RenderDocument(document, outputStream);
        return Task.CompletedTask;
    }

    public void RenderDocument(DocumentLayout document, Stream outputStream)
    {
        using var wordDocument = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document);
        
        var mainPart = wordDocument.AddMainDocumentPart();
        
        RenderStyles(document, mainPart);
        RenderBody(document, mainPart);
    }

    private void RenderStyles(DocumentLayout document, MainDocumentPart mainPart)
    {
        var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        stylesPart.Styles = _stylesRenderer.RenderStyles(document);
    }

    private void RenderBody(DocumentLayout document, MainDocumentPart mainPart)
    {
        var body = new Body();
        
        var pages = document.Pages;
        
        var numberingRenderer = new Lazy<DocxNumberingRenderer>(
            () => new DocxNumberingRenderer(document, mainPart),
            false
        );

        var hyperlinkRenderer = new Lazy<DocxHyperlinkRenderer>(
            () => new DocxHyperlinkRenderer(mainPart),
            false
        );

        var pageRenderer = new DocxPageRenderer(numberingRenderer, hyperlinkRenderer);
        
        for (var i = 0; i < pages.Count; i++)
        {
            var page = pages[i];
            var finalPage = i == pages.Count - 1;

            pageRenderer.RenderPage(page, body, finalPage);
        }
        
        mainPart.Document = new Document { Body = body };
    }
}