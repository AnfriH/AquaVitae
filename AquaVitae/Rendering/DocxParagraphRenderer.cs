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
            paragraphNode.ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = paragraph.Style.Value }
            };
        }
        parent.AppendChild(paragraphNode);
        return Task.CompletedTask;
    }
}