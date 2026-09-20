using AquaVitae.Layouts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Rendering;

public sealed class DocxDocumentRenderer(DocxPageRenderer pageRenderer, DocxStylesRenderer stylesRenderer) : IDocumentRenderer
{
    public async Task RenderAsync(DocumentLayout document, Stream outputStream)
    {
        using var wordDocument = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document);
        
        var mainPart = wordDocument.AddMainDocumentPart();
        
        await RenderStyles(document, mainPart);
        await RenderBody(document, mainPart);
    }

    private async Task RenderStyles(DocumentLayout document, MainDocumentPart mainPart)
    {
        var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        stylesPart.Styles = await stylesRenderer.RenderAsync(document);;
    }

    private async Task RenderBody(DocumentLayout document, MainDocumentPart mainPart)
    {
        var body = new Body();
        
        var pages = document.Pages;
        
        for (var i = 0; i < pages.Count; i++)
        {
            var page = pages[i];
            var finalPage = i == pages.Count - 1;

            await pageRenderer.RenderAsync(page, body, finalPage);
        }
        
        mainPart.Document = new Document { Body = body };
    }
}