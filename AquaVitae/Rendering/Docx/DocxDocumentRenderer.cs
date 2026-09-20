using AquaVitae.Layouts;
using AquaVitae.Rendering.Abstractions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Rendering.Docx;

public sealed class DocxDocumentRenderer(DocxPageRenderer pageRenderer, DocxStylesRenderer stylesRenderer) : IDocumentRenderer
{
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
        stylesPart.Styles = stylesRenderer.RenderStyles(document);
    }

    private void RenderBody(DocumentLayout document, MainDocumentPart mainPart)
    {
        var body = new Body();
        
        var pages = document.Pages;
        
        for (var i = 0; i < pages.Count; i++)
        {
            var page = pages[i];
            var finalPage = i == pages.Count - 1;

            pageRenderer.RenderPage(page, body, finalPage);
        }
        
        mainPart.Document = new Document { Body = body };
    }
}