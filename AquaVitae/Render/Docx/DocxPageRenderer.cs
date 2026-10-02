using System.Xml;
using AquaVitae.Layouts;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public sealed class DocxPageRenderer(
    DocxNumberingRenderer numberingRenderer,
    DocxHyperlinkRenderer hyperlinkRenderer,
    DocxStylesRenderer stylesRenderer,
    DocxSvgRenderer svgRenderer
)
{
    private DocxParagraphRenderer ParagraphRenderer => field ??= new DocxParagraphRenderer(hyperlinkRenderer, stylesRenderer);
    private DocxVerticalListRenderer VerticalListRenderer => field ??= new DocxVerticalListRenderer(numberingRenderer, ParagraphRenderer);
    
    public void RenderPage(PageLayout page, Body body, bool finalPage, XmlDocument? pageSvg)
    {
        var paragraphs = new List<ParagraphLayoutBase>();
        
        foreach (var element in page.PageElements)
        {
            switch (element)
            {
                case TextBoxLayout textBox:
                    // Direct passthrough to paragraphs. We don't insert boxes in the underlying layout text
                    // because MS Word breaks floating elements, and they end up smushed together on the left margin.
                    paragraphs.AddRange(textBox.Paragraphs);
                    break;
            }
        }

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

        if (pageSvg != null)
        {
            var drawing = svgRenderer.RenderSvg(page, pageSvg);
            var paragraph = body.AppendChild(new Paragraph());
            paragraph.AppendChild(new Run(drawing));
        }
        
        RenderPageFormatting(page, body, finalPage);
    }

    private static void RenderPageFormatting(PageLayout page, Body body, bool finalPage)
    {
        var sectionProperties = new SectionProperties();
        
        // Enforce a page break
        var sectionType = new SectionType { Val = SectionMarkValues.NextPage };
        sectionProperties.AppendChild(sectionType);
        
        // Set page size
        var (width, height) = page.PageSize;
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