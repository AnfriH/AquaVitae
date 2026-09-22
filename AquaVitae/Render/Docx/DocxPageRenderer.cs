using AquaVitae.Layouts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Rendering.Docx;

public sealed class DocxPageRenderer(
    Lazy<DocxNumberingRenderer> numberingRenderer,
    Lazy<DocxHyperlinkRenderer> hyperlinkRenderer
)
{
    private DocxBoxRenderer BoxRenderer => field ??= new DocxBoxRenderer(numberingRenderer, hyperlinkRenderer);
    
    public void RenderPage(PageLayout page, Body body, bool finalPage)
    {
        var fillColor = page.FillColor;
        if (fillColor != null)
        {
            var box = BoxRenderer.RenderBox(
                new BoxLayout(0, 0, page.PageSize.Width, page.PageSize.Height)
                {
                    FillColor = fillColor.Value
                }
            );
            body.AppendChild(box);
        }
        
        foreach (var element in page.PageElements)
        {
            OpenXmlElement child = element switch
            {
                BoxLayout box => BoxRenderer.RenderBox(box)
            };

            body.AppendChild(child);
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
        // TODO: Make customisable
        var pageMargin = new PageMargin
        {
            Top = 0,
            Left = 0,
            Right = 0,
            Bottom = 0,
            Header = 0,
            Footer = 0,
            Gutter = 0
        };
        sectionProperties.AppendChild(pageMargin);
        
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