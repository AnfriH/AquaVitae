using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class HyperlinkLayout(
    Uri uri,
    string text,
    StyleId<RunStyle> style
) : RunLayoutBase(text, style)
{
    public HyperlinkLayout(Uri uri, StyleId<RunStyle> style) : 
        this(uri, uri.ToString(), style) { }
    
    public Uri Uri => uri;
}