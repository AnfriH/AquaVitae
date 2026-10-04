using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;
using Margins = VectSharp.Markdown.Margins;

namespace AquaVitae.Render.Vector;

public class VectorTextBoxRenderer(
    VectorStyleRenderer styleRenderer,
    VectorHyperlinkRenderer hyperlinkRenderer
)
{
    public PrintPoint RenderTextBox(TextBoxLayout textBoxLayout, Graphics graphics, PrintPoint width)
    {
        var textGraphics = textBoxLayout.Paragraphs.Count > 0 
            ? RenderParagraphs(textBoxLayout, width) 
            : null;

        var actualHeight = textGraphics != null 
            ? new PrintPoint((float)textGraphics.Height) 
            : PrintPoint.Zero;
        
        if (textBoxLayout.FillColor.HasValue)
        {
            var fillColor = textBoxLayout.FillColor.Value;
            
            graphics.FillRectangle(
                0,
                0,
                width.Points,
                actualHeight.Points,
                fillColor.ToVectSharpColor()
            );
        }
        
        if (textGraphics != null)
        {
            graphics.DrawGraphics(0, 0, textGraphics.Graphics);
        }

        return actualHeight;
    }

    private Page RenderParagraphs(TextBoxLayout textBoxLayout, PrintPoint width)
    {
        var margins = textBoxLayout.InnerMargins;
        
        var paragraphRenderer = new VectorParagraphRenderer(styleRenderer)
        {
            Margins = new Margins(
                margins.Left.Points,
                margins.Top.Points,
                margins.Right.Points,
                margins.Bottom.Points
            ),
            SpaceAfterParagraph = textBoxLayout.ParagraphSpacing.Points
        };
        var markdownRenderer = new VectorMarkdownRenderer(paragraphRenderer, styleRenderer);

        var document = markdownRenderer.RenderToDocument(textBoxLayout.Paragraphs);
        
        var innerPage = paragraphRenderer.RenderSinglePage(
            document,
            width.Points,
            out var hyperlinks,
            out _
        );
        
        hyperlinkRenderer.AddHyperlinks(hyperlinks);
        return innerPage;
    }
}