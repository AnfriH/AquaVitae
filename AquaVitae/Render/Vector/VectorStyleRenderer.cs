using System.Text;
using AquaVitae.Common;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorStyleRenderer(StyleLayout styleLayout)
{
    private readonly Dictionary<string, FontFamily> _fontFamilies = new();
    private readonly Dictionary<string, Font> _fonts = new();

    public TStyle GetStyle<TStyle>(StyleId<TStyle> id) where TStyle : IStyle
    {
        return styleLayout.GetStyle(id);
    }
    
    public Font GetFont(StyleId<RunStyle> id)
    {
        if (id.IsNone) throw new ArgumentNullException(nameof(id));
        if (_fonts.TryGetValue(id.Value, out var font)) return font;
        
        var runStyle = styleLayout.GetStyle(id);
        var fontStyle = styleLayout.GetStyle(runStyle.Font);

        var fontPath = (runStyle.Bold, runStyle.Italic) switch
        {
            (false, false) => fontStyle.Regular,
            (true, false) => fontStyle.Bold,
            (false, true) => fontStyle.Italic,
            (true, true) => fontStyle.BoldItalic
        };
        
        if (fontPath == null) throw new NullReferenceException(nameof(fontPath));
        
        if (!_fontFamilies.TryGetValue(fontPath, out var fontFamily))
        {
            fontFamily = FontFamily.DefaultFontLibrary.ResolveFontFamily(fontPath);
            _fontFamilies[fontPath] = fontFamily;
        }
        
        font = new Font(fontFamily, runStyle.FontSize.Points, runStyle.Underline);
        _fonts[id.Value] = font;
        
        return font;
    }
}