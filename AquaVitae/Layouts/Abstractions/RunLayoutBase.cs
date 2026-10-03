using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts.Abstractions;

public closed class RunLayoutBase(string text, StyleId<RunStyle> style) : LayoutBase
{
    public string Text => text;
    public StyleId<RunStyle> Style => style;
}