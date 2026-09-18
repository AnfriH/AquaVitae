using AquaVitae.Layouts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Rendering;

public sealed class DocxDocumentRenderer(DocxPageRenderer pageRenderer) : IDocumentRenderer
{
    public async Task RenderAsync(DocumentLayout document, Stream outputStream)
    {
        using var wordDocument = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document);
        
        var mainPart = wordDocument.AddMainDocumentPart();

        mainPart.Document = new Document
        {
            Body = new Body()
        };
        
        await RenderDocument(document, mainPart.Document.Body);
        
        mainPart.Document.Save();
    }

    private async Task RenderDocument(DocumentLayout document, Body body)
    {
        var pages = document.Pages;
        
        for (var i = 0; i < pages.Count; i++)
        {
            var page = pages[i];
            var finalPage = i == pages.Count - 1;

            await pageRenderer.RenderAsync(page, body, finalPage);
        }
    }
}