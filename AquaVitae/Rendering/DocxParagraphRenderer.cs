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
            var runNode = new Run(new Text(run.Text));
            paragraphNode.AppendChild(runNode);
        }
        parent.AppendChild(paragraphNode);
        return Task.CompletedTask;
    }
}