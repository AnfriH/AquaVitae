using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class HyperlinkLayout(
    Uri uri,
    StyleId<RunStyle> style,
    string? text = null
) : RunLayoutBase(text ?? uri.ToString(), style)
{
    public Uri Uri => uri;
}