using AquaVitae.Layouts.Styles;
using Markdig.Syntax.Inlines;
using VectSharp;
using VectSharp.Markdown;

namespace AquaVitae.Render.Vector;

public sealed class ParagraphRenderer(VectorStyleRenderer styleRenderer) : MarkdownRenderer
{
    private readonly Dictionary<Inline, StyleId<RunStyle>> _inlineStyles = new(ReferenceEqualityComparer.Instance);

    public void Clear()
    {
        _inlineStyles.Clear();
    }

    public void StyleInline(Inline inline, StyleId<RunStyle> styleId)
    {
        _inlineStyles.Add(inline, styleId);
    }
    
    protected override void OnInlineRendering(
        ref MarkdownContext context,
        ref Graphics graphics,
        ref Inline inline
    )
    {
        base.OnInlineRendering(ref context, ref graphics, ref inline);
        if (!_inlineStyles.TryGetValue(inline, out var styleId)) return;
        
        // Rather than using the actual Markdown syntax, this renderer allows us to
        // take full control over how the renderer handles text. This lets us do a
        // bunch of styling that the built-in Markdown renderer does not expose to us!
        var style = styleRenderer.GetStyle(styleId);
        var font = styleRenderer.GetFont(styleId);
        
        context.Font = font;
        context.StrikeThrough = style.Strikethrough;
        context.Colour = style.Color.ToVectSharpColor();
    }
}