using AquaVitae.Layouts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Rendering;

public class DocxParagraphRenderer
{
    public Task RenderAsync(ParagraphLayout paragraph, OpenXmlCompositeElement parent)
    {
        var paragraphNode = new Paragraph();
        foreach (var run in paragraph.Runs)
        {
            var runNode = paragraphNode.AppendChild(new Run(new Text(run.Text)));
            if (run.StyleId != null)
            {
                runNode.RunProperties = new RunProperties
                {
                    RunStyle = new RunStyle { Val = run.StyleId }
                };
            }
        }

        if (paragraph.StyleId != null)
        {
            paragraphNode.ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = paragraph.StyleId }
            };
        }
        parent.AppendChild(paragraphNode);
        return Task.CompletedTask;
    }
}