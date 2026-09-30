using System.Runtime.InteropServices.ComTypes;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;
using AquaVitae.Layouts.Types;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using VectSharp;
using Margins = VectSharp.Markdown.Margins;

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
        
        if (textBoxLayout.Paragraphs.Count > 0) RenderParagraphs(textBoxLayout, graphics);
    }

    private void RenderParagraphs(TextBoxLayout textBoxLayout, Graphics graphics)
    {
        var margins = textBoxLayout.InnerMargins;
        
        paragraphRenderer.Margins = new Margins(
            margins.Left.Points,
            margins.Top.Points,
            margins.Right.Points,
            margins.Bottom.Points
        );
        
        var document = new MarkdownDocument();
        foreach (var paragraphLayout in textBoxLayout.Paragraphs)
        {
            document.Add(RenderParagraphBase(paragraphLayout));
        }
        
        var innerPage = paragraphRenderer.RenderSinglePage(
            document,
            textBoxLayout.Width.Points,
            out _,
            out _
        );
        
        paragraphRenderer.Clear();
        
        graphics.DrawGraphics(
            new Point(textBoxLayout.X.Points, textBoxLayout.Y.Points),
            innerPage.Graphics
        );
    }

    private Block RenderParagraphBase(ParagraphLayoutBase paragraphLayoutBase)
    {
        Block block = paragraphLayoutBase switch
        {
            ParagraphLayout paragraphLayout => RenderParagraph(paragraphLayout),
            VerticalListLayout verticalListLayout => RenderVerticalList(verticalListLayout)
        };
        paragraphRenderer.StyleParagraph(block, paragraphLayoutBase.Style);
        return block;
    }

    private ListBlock RenderVerticalList(VerticalListLayout verticalListLayout)
    {
        var listStyleId = verticalListLayout.ListStyle;
        var listStyle = styleRenderer.GetStyle(listStyleId);
        var parser = new ListBlockParser();
        var list = new ListBlock(parser)
        {
            IsOrdered = listStyle.Ordered
        };
        
        paragraphRenderer.StyleList(list, listStyleId);

        var i = 1;
        foreach (var paragraphLayout in verticalListLayout.Paragraphs)
        {
            var listItem = new ListItemBlock(parser)
            {
                Order = i++,
            };
            listItem.Add(RenderParagraphBase(paragraphLayout));
            list.Add(listItem);
        }
        
        return list;
    }

    private ParagraphBlock RenderParagraph(ParagraphLayout paragraphLayout)
    {
        // var paragraphStyle = styleRenderer.GetStyle(paragraphLayout.Style);
        
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
            
            paragraphRenderer.StyleRun(inline, runLayoutBase.Style);
            container.AppendChild(inline);
        }

        return paragraph;
    }
}