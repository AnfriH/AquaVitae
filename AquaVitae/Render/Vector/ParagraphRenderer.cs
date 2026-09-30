using AquaVitae.Layouts.Styles;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using VectSharp;
using VectSharp.Markdown;

namespace AquaVitae.Render.Vector;

public sealed class ParagraphRenderer(VectorStyleRenderer styleRenderer) : MarkdownRenderer
{
    private readonly Dictionary<Inline, StyleId<RunStyle>> _runStyles = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<Block, StyleId<ParagraphStyle>> _paragraphStyles = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<Block, StyleId<VerticalListStyle>> _verticalListStyles = new(ReferenceEqualityComparer.Instance);
    
    public void Clear()
    {
        _runStyles.Clear();
        _paragraphStyles.Clear();
    }

    public void StyleRun(Inline inline, StyleId<RunStyle> styleId)
    {
        _runStyles.Add(inline, styleId);
    }

    public void StyleParagraph(Block block, StyleId<ParagraphStyle> styleId)
    {
        _paragraphStyles.Add(block, styleId);
    }

    public void StyleList(Block block, StyleId<VerticalListStyle> styleId)
    {
        _verticalListStyles.Add(block, styleId);
    }
    
    protected override void OnInlineRendering(
        ref MarkdownContext context,
        ref Graphics graphics,
        ref Inline inline
    )
    {
        base.OnInlineRendering(ref context, ref graphics, ref inline);
        if (!_runStyles.TryGetValue(inline, out var styleId)) return;
        
        // Rather than using the actual Markdown syntax, this renderer allows us to
        // take full control over how the renderer handles text. This lets us do a
        // bunch of styling that the built-in Markdown renderer does not expose to us!
        var style = styleRenderer.GetStyle(styleId);
        var font = styleRenderer.GetFont(styleId);
        
        context.Font = font;
        context.StrikeThrough = style.Strikethrough;
        context.Colour = style.Color.ToVectSharpColor();
    }

    protected override void OnBlockRendering(ref MarkdownContext context, ref Graphics graphics, ref Block block)
    {
        base.OnBlockRendering(ref context, ref graphics, ref block);
        if (_paragraphStyles.TryGetValue(block, out var paraId))
        {
            var paraStyle = styleRenderer.GetStyle(paraId);
            SpaceAfterLine = paraStyle.LineSpacing.Points;
        }

        if (_verticalListStyles.TryGetValue(block, out var vertId))
        {
            var vertStyle = styleRenderer.GetStyle(vertId);
            context.Colour = vertStyle.ElementColor.ToVectSharpColor();
            IndentWidth = vertStyle.Indent.Points;
        }
    }
}