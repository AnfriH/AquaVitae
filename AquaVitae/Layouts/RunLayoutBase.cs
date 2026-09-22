using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public closed class RunLayoutBase(string text) : LayoutBase
{
    public string Text => text;
    public StyleId<RunLayoutBase> Style { get; init; }
}