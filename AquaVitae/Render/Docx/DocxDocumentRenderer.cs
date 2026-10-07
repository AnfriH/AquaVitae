using AquaVitae.Layouts;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Render.Vector;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public sealed class DocxDocumentRenderer(DocxRendererSettings settings)
{
    public void RenderDocument(DocumentLayout document, SvgDocument svgDocument, Stream outputStream)
    {
        using var wordDocument = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document);
        
        var mainPart = wordDocument.AddMainDocumentPart();
        
        RenderBody(document, mainPart, svgDocument.Pages);
    }

    private void RenderBody(DocumentLayout document, MainDocumentPart mainPart, IReadOnlyList<SvgPage> svgPages)
    {
        var body = new Body();
        
        var numberingRenderer = new DocxNumberingRenderer(document, mainPart);
        var hyperlinkRenderer = new DocxHyperlinkRenderer(mainPart);
        var stylesRenderer = new DocxStylesRenderer(document, mainPart, settings);
        var svgRenderer = new DocxDrawingRenderer(mainPart, hyperlinkRenderer);
        
        var pageRenderer = new DocxPageRenderer(
            numberingRenderer, 
            hyperlinkRenderer, 
            stylesRenderer, 
            svgRenderer,
            settings
        );
        
        var paragraphs = new List<ParagraphLayoutBase>();
        document.CollectParagraphs(paragraphs.Add);
        
        // We don't use List.Sort here, as it's not stable and can reorder paragraphs with the same priority group.
        var orderedParagraphs = paragraphs.Order().ToList();

        for (var i = 0; i < svgPages.Count; i++)
        {
            var svgPage = svgPages[i];
            pageRenderer.RenderPage(body, svgPage, orderedParagraphs, svgPages.Count, i);
        }
        
        mainPart.Document = new Document { Body = body };
    }
}