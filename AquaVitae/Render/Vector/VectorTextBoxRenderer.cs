using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using VectSharp;

namespace AquaVitae.Render.Vector;

public class VectorTextBoxRenderer(
    ParagraphRenderer paragraphRenderer,
    VectorStyleRenderer styleRenderer
)
{
    public void RenderTextBox(TextBoxLayout textBoxLayout, Graphics graphics)
    {
        if (textBoxLayout.FillColor.HasValue)
        {
            var fillColor = textBoxLayout.FillColor.Value;
            
            graphics.FillRectangle(
                textBoxLayout.X.Points,
                textBoxLayout.Y.Points,
                textBoxLayout.Width.Points,
                textBoxLayout.Height.Points,
                fillColor.ToVectSharpColor()
            );
        }
        
        RenderParagraphs(textBoxLayout, graphics);
    }

    private void RenderParagraphs(TextBoxLayout textBoxLayout, Graphics graphics)
    {
        var x = textBoxLayout.X.Points;
        var y = textBoxLayout.Y.Points;
        
        for (var i = 0; i < textBoxLayout.Paragraphs.Count; i++)
        {
            var paragraphLayoutBase = textBoxLayout.Paragraphs[i];
            var page = paragraphLayoutBase switch
            {
                ParagraphLayout paragraphLayout => RenderParagraph(
                    paragraphLayout,
                    textBoxLayout.Width
                ),
                VerticalListLayout verticalListLayout => throw new NotImplementedException()
            };
            
            var pageGraphics = page.Graphics;
            
            graphics.DrawGraphics(new Point(x, y), pageGraphics);
            y += textBoxLayout.Height.Points;
        }
    }

    private Page RenderParagraph(
        ParagraphLayout paragraphLayout,
        PrintPoint width
    )
    {
        // var paragraphStyle = styleRenderer.GetStyle(paragraphLayout.Style);
        
        var document = new MarkdownDocument();
        var container = new ContainerInline();
        var paragraph = new ParagraphBlock { Inline = container };
        
        foreach (var runLayoutBase in paragraphLayout.Runs)
        {
            Inline inline = runLayoutBase switch
            {
                HyperlinkLayout hyperlinkLayout => new LinkInline(
                    hyperlinkLayout.Uri.ToString(),
                    hyperlinkLayout.Text
                ),
                RunLayout runLayout => new LiteralInline(runLayout.Text)
            };
            
            paragraphRenderer.StyleInline(inline, runLayoutBase.Style);
            container.AppendChild(inline);
        }
        
        document.Add(paragraph);
        
        var innerPageContainer = paragraphRenderer.RenderSinglePage(
            document,
            width.Points,
            out var linkDestinations,
            out _
        );
        
        paragraphRenderer.Clear();

        return innerPageContainer;
    }
}