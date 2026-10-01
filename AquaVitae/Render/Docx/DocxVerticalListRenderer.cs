using AquaVitae.Layouts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public sealed class DocxVerticalListRenderer(DocxNumberingRenderer numberingRenderer, DocxParagraphRenderer paragraphRenderer)
{
    public void RenderVerticalList(VerticalListLayout verticalList, OpenXmlCompositeElement parentElement, int level = 0)
    {
        if (verticalList.Paragraphs.Count == 0) return;
        
        var numberingId = numberingRenderer.GetNumberingId(verticalList.ListStyle, level);

        foreach (var child in verticalList.Paragraphs)
        {
            switch (child)
            {
                case VerticalListLayout subList:
                    RenderVerticalList(subList, parentElement, level + 1);
                    break;
                case ParagraphLayout paragraph:
                {
                    var paragraphElement = paragraphRenderer.RenderParagraph(paragraph);
                    paragraphElement.ParagraphProperties ??= new ParagraphProperties();

                    var properties = paragraphElement.ParagraphProperties;
                    properties.NumberingProperties = new NumberingProperties
                    {
                        NumberingId = new NumberingId { Val = numberingId },
                        NumberingLevelReference = new NumberingLevelReference { Val = level }
                    };
                    
                    parentElement.AppendChild(paragraphElement);
                    break;
                }
            }
        }
    }
}