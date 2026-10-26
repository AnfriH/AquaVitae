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
    public PrintPoint RenderTextBox(TextBoxLayout textBoxLayout, Graphics graphics, LayoutConstraints constraints)
    {
        var textGraphics = textBoxLayout.Paragraphs.Count > 0 
            ? RenderParagraphs(textBoxLayout, constraints) 
            : null;

        var actualHeight = textGraphics != null 
            ? new PrintPoint((float)textGraphics.Height) 
            : PrintPoint.Zero;
        
        if (textBoxLayout.FillColor.HasValue)
        {
            var fillColor = textBoxLayout.FillColor.Value;
            
            graphics.FillRectangle(
                constraints.X.Points,
                constraints.Y.Points,
                constraints.Width.Points,
                actualHeight.Points,
                fillColor.ToVectSharpColor()
            );
        }
        
        if (textGraphics != null)
        {
            graphics.DrawGraphics(
                constraints.X.Points,
                constraints.Y.Points,
                textGraphics.Graphics
            );
        }

        return actualHeight;
    }

    private Page RenderParagraphs(TextBoxLayout textBoxLayout, LayoutConstraints constraints)
    {
        var margins = textBoxLayout.InnerMargins;
        
        var paragraphRenderer = new VectorParagraphRenderer(styleRenderer, constraints.X, constraints.Y)
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
            constraints.Width.Points,
            out var hyperlinks,
            out _
        );
        
        hyperlinkRenderer.AddLinkBounds(paragraphRenderer.Links);
        hyperlinkRenderer.AddHyperlinks(hyperlinks);
        return innerPage;
    }
}