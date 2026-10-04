using AquaVitae.Layouts;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;

namespace AquaVitae.Render.Docx;

public class DocxParagraphRenderer(
    DocxHyperlinkRenderer hyperlinkRenderer,
    DocxStylesRenderer stylesRenderer,
    DocxRendererSettings settings
)
{
    public Paragraph RenderParagraph(ParagraphLayout paragraph)
    {
        var paragraphElement = new Paragraph();
        foreach (var runBase in paragraph.Runs)
        {
            OpenXmlElement element = runBase switch
            {
                RunLayout run => CreateRun(run),
                HyperlinkLayout hyperlink => CreateHyperlink(hyperlink)
            };
            
            paragraphElement.AppendChild(element);
        }

        stylesRenderer.AddParagraphStyle(paragraph.Style);
            
        paragraphElement.ParagraphProperties = new ParagraphProperties
        {
            ParagraphStyleId = new ParagraphStyleId { Val = paragraph.Style.Value }
        };
        
        return paragraphElement;
    }

    private Run CreateRun(RunLayoutBase layout)
    {
        stylesRenderer.AddRunStyle(layout.Style);
        
        var runElement = new Run(new Text(layout.Text))
        {
            RunProperties = new RunProperties
            {
                RunStyle = new RunStyle { Val = layout.Style.Value }
            }
        };
        return runElement;
    }

    private Hyperlink CreateHyperlink(HyperlinkLayout layout)
    {
        var relId = hyperlinkRenderer.AddHyperlink(layout.Uri);
        
        var hyperlinkElement = new Hyperlink
        {
            Id = relId,
        };

        var run = CreateRun(layout);

        if (!settings.IncludeTextColor)
        {
            var runProperties = run.RunProperties!;
            runProperties.Color = new Color { Val = Colors.Blue.ToString(ColorFormats.Rgb) };
            runProperties.Underline = new Underline { Val = UnderlineValues.Single };
        }
        
        hyperlinkElement.AppendChild(run);
        
        return hyperlinkElement;
    }
}