using AquaVitae.Layouts;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Rendering.Docx;

public class DocxParagraphRenderer
{
    public Paragraph RenderParagraph(ParagraphLayout paragraph)
    {
        var paragraphElement = new Paragraph();
        foreach (var run in paragraph.Runs)
        {
            var runNode = paragraphElement.AppendChild(new Run(new Text(run.Text)));
            if (run.Style.Value != null)
            {
                runNode.RunProperties = new RunProperties
                {
                    RunStyle = new RunStyle { Val = run.Style.Value }
                };
            }
        }

        if (paragraph.Style.Value != null)
        {
            paragraphElement.ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = paragraph.Style.Value }
            };
        }
        
        return paragraphElement;
    }
}