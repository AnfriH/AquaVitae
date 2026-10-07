using AquaVitae.Layouts;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;
using AquaVitae.Render.Vector;
using DocumentFormat.OpenXml.Wordprocessing;
using PageSize = DocumentFormat.OpenXml.Wordprocessing.PageSize;

namespace AquaVitae.Render.Docx;

public sealed class DocxPageRenderer(
    DocxNumberingRenderer numberingRenderer,
    DocxHyperlinkRenderer hyperlinkRenderer,
    DocxStylesRenderer stylesRenderer,
    DocxDrawingRenderer drawingRenderer,
    DocxRendererSettings settings
)
{
    private static readonly PrintPoint TopMargin = PrintPoint.FromInches(0.5f);
    
    private DocxParagraphRenderer ParagraphRenderer => field ??= new DocxParagraphRenderer(hyperlinkRenderer, stylesRenderer, settings);
    private DocxVerticalListRenderer VerticalListRenderer => field ??= new DocxVerticalListRenderer(numberingRenderer, ParagraphRenderer);
    
    public void RenderPage(
        Body body,
        SvgPage svgPage,
        IReadOnlyList<ParagraphLayoutBase> contents,
        int pageCount,
        int pageIndex
    )
    {
        var isFirstPage = pageIndex == 0;
        var isLastPage = pageIndex == pageCount - 1;
        
        RenderVisualContents(body, svgPage);
        
        if (isFirstPage) RenderFallback(body, svgPage.Width - TopMargin);
        
        
        if (isLastPage) RenderTextualContents(body, contents);
        
        RenderPageFormatting(body, svgPage, isLastPage);
    }
    
    private void RenderVisualContents(Body body, SvgPage svgPage)
    {
        var svgParagraph = body.AppendChild(new Paragraph());

        svgParagraph.AppendChild(new Run(drawingRenderer.CreateSvgPanel(svgPage, 50)));

        foreach (var linkPosition in svgPage.Hyperlinks)
        {
            svgParagraph.AppendChild(new Run(drawingRenderer.CreateHyperlinkClickBox(linkPosition, 100)));
        }
    }

    private void RenderTextualContents(Body body, IReadOnlyList<ParagraphLayoutBase> contents)
    {
        foreach (var paragraphLayout in contents)
        {
            switch (paragraphLayout)
            {
                case ParagraphLayout paragraph:
                    body.AppendChild(ParagraphRenderer.RenderParagraph(paragraph));
                    break;
                case VerticalListLayout vertList:
                    VerticalListRenderer.RenderVerticalList(vertList, body);
                    break;
            }
        }
    }

    private void RenderFallback(Body body, PrintPoint width)
    {
        body.AppendChild(new Paragraph())
            .AppendChild(new Run())
            .AppendChild(
                drawingRenderer.RenderFallbackDrawing(width)
            );
    }

    private static void RenderPageFormatting(Body body, SvgPage svgPage, bool isLastPage)
    {
        var sectionProperties = new SectionProperties();
        
        // Enforce a page break
        var sectionType = new SectionType { Val = SectionMarkValues.NextPage };
        sectionProperties.AppendChild(sectionType);
        
        // Set page size
        var pageSize = new PageSize
        {
            Width = svgPage.Width.ToTwipsUInt(),
            Height = svgPage.Height.ToTwipsUInt()
        };
        sectionProperties.AppendChild(pageSize);
        
        sectionProperties.AppendChild(new PageMargin
        {
            Bottom = 0,
            Top = TopMargin.ToTwipsInt(),
            Left = 0,
            Right = 0,
            Header = 0,
            Footer = 0,
            Gutter = 0
        });
        
        // Attach a page break to the last paragraph. If there are none (somehow), we'll create one.
        if (body.LastChild is not Paragraph paragraph) paragraph = body.AppendChild(new Paragraph());
        
        // If it's the last page of the document, the section properties must be added to the body directly.
        if (isLastPage)
        {
            body.AppendChild(sectionProperties);
            return;
        }

        // Elsewise, we add it to the paragraph along with a page break
        paragraph.AppendChild(new Run(new Break { Type = BreakValues.Page }));
        paragraph.ParagraphProperties = new ParagraphProperties { SectionProperties = sectionProperties };
    }
}