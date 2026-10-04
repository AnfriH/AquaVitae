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
        var pages = document.Pages;
        
        var numberingRenderer = new DocxNumberingRenderer(document, mainPart);
        var hyperlinkRenderer = new DocxHyperlinkRenderer(mainPart);
        var stylesRenderer = new DocxStylesRenderer(document, mainPart, settings);
        var svgRenderer = new DocxSvgRenderer(mainPart, hyperlinkRenderer);
        
        var pageRenderer = new DocxPageRenderer(
            numberingRenderer, 
            hyperlinkRenderer, 
            stylesRenderer, 
            svgRenderer,
            settings
        );
        
        var paragraphs = new List<ParagraphLayoutBase>();
        document.CollectParagraphs(paragraphs.Add);
        var orderedParagraphs = paragraphs.Order().ToList();

        var vectorStyleRenderer = new VectorStyleRenderer(document.Styles);
        var paragraphsByPage = DocxTextMeasurer.GetParagraphLayoutsByPage(
            orderedParagraphs,
            vectorStyleRenderer,
            svgPages
        );

        for (var i = 0; i < svgPages.Count; i++)
        {
            var svgPage = svgPages[i];
            IReadOnlyList<ParagraphLayoutBase> pageParagraphs = paragraphsByPage.Count > i 
                ? paragraphsByPage[i] 
                : Array.Empty<ParagraphLayoutBase>();
            
            pageRenderer.RenderPage(body, svgPage, pageParagraphs, i == svgPages.Count - 1);
        }
        
        mainPart.Document = new Document { Body = body };
    }
}