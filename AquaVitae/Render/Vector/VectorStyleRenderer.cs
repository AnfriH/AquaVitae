using System.Text;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorStyleRenderer(DocumentLayout documentLayout)
{
    private readonly Dictionary<string, Font> _fontLookup = new();

    // TODO: load TTF files as requested
    private readonly IFontLibrary _fontLibrary = FontFamily.DefaultFontLibrary;
    
    public Font GetFont(StyleId<RunLayoutBase> runStyleId)
    {
        if (runStyleId.IsNone) throw new NotImplementedException("No handling for default styles yet");
        if (_fontLookup.TryGetValue(runStyleId.Value, out var font)) return font;
        var style = GetRunStyle(runStyleId);

        var sb = new StringBuilder(style.FontName!);

        if (style.Bold) sb.Append(" Bold");
        if (style.Italic) sb.Append(" Italic");
        
        var fontFamily = _fontLibrary.ResolveFontFamily(sb.ToString());
        
        
        font = new Font(fontFamily, style.FontSize!.Value.Points, style.Underline);
        _fontLookup[runStyleId.Value] = font;
        return font;
    }

    public RunStyle GetRunStyle(StyleId<RunLayoutBase> runStyleId)
    {
        return documentLayout.RunStyles[runStyleId.Value!];
    }

    public ParagraphStyle GetParagraphStyle(StyleId<ParagraphLayout> paragraphStyleId)
    {
        return documentLayout.ParagraphStyles[paragraphStyleId.Value!];
    }
}