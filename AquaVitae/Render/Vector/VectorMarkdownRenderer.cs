using AquaVitae.Layouts;
using AquaVitae.Layouts.Abstractions;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace AquaVitae.Render.Vector;

public sealed class VectorMarkdownRenderer(VectorParagraphRenderer paragraphRenderer, VectorStyleRenderer styleRenderer)
{
    public MarkdownDocument RenderToDocument(IEnumerable<ParagraphLayoutBase> paragraphLayouts)
    {
        var document = new MarkdownDocument();
        foreach (var paragraphLayoutBase in paragraphLayouts)
        {
            var block = RenderParagraphBase(paragraphLayoutBase);
            document.Add(block);
        }
        return document;
    }
    
    private Block RenderParagraphBase(ParagraphLayoutBase paragraphLayoutBase)
    {
        Block block = paragraphLayoutBase switch
        {
            ParagraphLayout paragraphLayout => RenderParagraph(paragraphLayout),
            VerticalListLayout verticalListLayout => RenderVerticalList(verticalListLayout)
        };
        paragraphRenderer.StyleParagraph(block, paragraphLayoutBase);
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
        
        paragraphRenderer.StyleList(list, verticalListLayout);

        var i = 1;
        foreach (var paragraphLayout in verticalListLayout.Paragraphs)
        {
            var listItem = new ListItemBlock(parser)
            {
                Order = i++
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
                ).AppendChild(new LiteralInline(hyperlinkLayout.Text)),
                RunLayout runLayout => new LiteralInline(runLayout.Text)
            };
            
            paragraphRenderer.StyleRun(inline, runLayoutBase);
            container.AppendChild(inline);
        }

        return paragraph;
    }
}