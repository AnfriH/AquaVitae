using AquaVitae.Layouts;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public sealed class DocxBoxRenderer(
    DocxNumberingRenderer numberingRenderer,
    DocxHyperlinkRenderer hyperlinkRenderer,
    DocxStylesRenderer stylesRenderer
)
{
    private DocxParagraphRenderer ParagraphRenderer => field ??= new DocxParagraphRenderer(
        hyperlinkRenderer,
        stylesRenderer
    );
    
    private DocxVerticalListRenderer VerticalListRenderer => field ??= new DocxVerticalListRenderer(
        numberingRenderer,
        ParagraphRenderer
    );
    
    public void RenderBox(TextBoxLayout textBox, Body body)
    {
        // Direct passthrough to paragraphs. We don't insert boxes in the underlying layout text
        // because MS Word breaks floating elements, and they end up smushed together on the left margin.
        foreach (var child in textBox.Paragraphs)
        {
            switch (child)
            {
                case ParagraphLayout paragraph:
                    body.AppendChild(ParagraphRenderer.RenderParagraph(paragraph));
                    break;
                case VerticalListLayout vertList:
                    VerticalListRenderer.RenderVerticalList(vertList, body);
                    break;
            }
        }
    }
}