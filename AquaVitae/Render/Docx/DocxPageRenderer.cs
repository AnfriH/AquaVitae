using System.Xml;
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
    DocxSvgRenderer svgRenderer,
    DocxRendererSettings settings
)
{
    private DocxParagraphRenderer ParagraphRenderer => field ??= new DocxParagraphRenderer(hyperlinkRenderer, stylesRenderer, settings);
    private DocxVerticalListRenderer VerticalListRenderer => field ??= new DocxVerticalListRenderer(numberingRenderer, ParagraphRenderer);
    
    public void RenderPage(Body body, SvgPage svgPage, IReadOnlyList<ParagraphLayoutBase> paragraphs, bool finalPage)
    {
        foreach (var paragraphLayout in paragraphs.OrderBy(p => p))
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
        
        var drawings = svgRenderer.RenderSvg(svgPage);
        var svgParagraph = body.AppendChild(new Paragraph());
        svgParagraph.Append(drawings.Select(d => new Run(d)));
        
        RenderPageFormatting(svgPage.Width, svgPage.Height, body, finalPage);
    }

    private static void RenderPageFormatting(PrintPoint width, PrintPoint height, Body body, bool finalPage)
    {
        var sectionProperties = new SectionProperties();
        
        // Enforce a page break
        var sectionType = new SectionType { Val = SectionMarkValues.NextPage };
        sectionProperties.AppendChild(sectionType);
        
        // Set page size
        var pageSize = new PageSize
        {
            Width = width.ToTwipsUInt(),
            Height = height.ToTwipsUInt()
        };
        sectionProperties.AppendChild(pageSize);
        
        // Set margin sizes to 0
        // var pageMargin = new PageMargin
        // {
        //     Top = 0,
        //     Left = 0,
        //     Right = 0,
        //     Bottom = 0,
        //     Header = 0,
        //     Footer = 0,
        //     Gutter = 0
        // };
        // sectionProperties.AppendChild(pageMargin);
        
        // We include a final paragraph to ensure that every page has at least one non-floating element.
        // Without this, the layout engine tends to munge the last two pages together.
        if (body.LastChild is not Paragraph paragraph) paragraph = body.AppendChild(new Paragraph());
        
        // If it's the last page of the document, the section properties must be added to the body directly.
        if (finalPage)
        {
            body.AppendChild(sectionProperties);
            return;
        }

        paragraph.ParagraphProperties = new ParagraphProperties { SectionProperties = sectionProperties };
    }
}