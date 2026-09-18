using System.Diagnostics;
using AquaVitae.Layouts;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Rendering;

public sealed class DocxPageRenderer(DocxBoxRenderer boxRenderer)
{
    public async Task RenderAsync(PageLayout page, Body body, bool finalPage)
    {
        foreach (var element in page.PageElements)
        {
            switch (element)
            {
                case BoxLayout box:
                    await boxRenderer.RenderAsync(box, body);
                    break;
                default:
                    throw new UnreachableException();
            }
        }
        
        RenderPageFormatting(page, body, finalPage);
    }
    
    private void RenderPageFormatting(PageLayout page, Body body, bool finalPage)
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
        
        // Final page has the section properties directly embedded into the body
        if (finalPage)
        {
            body.AppendChild(sectionProperties);
            return;
        }

        var paragraphProperties = new ParagraphProperties
        {
            SectionProperties = sectionProperties
        };

        // If an existing paragraph isn't already present, we create one
        if (body.LastChild is not Paragraph paragraph)
        {
            paragraph = new Paragraph();
            body.AppendChild(paragraph);
        }
        
        paragraph.ParagraphProperties = paragraphProperties;
    }
}