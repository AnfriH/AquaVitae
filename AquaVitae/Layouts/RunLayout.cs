using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class RunLayout(string text) : ILayout
{
    public string Text => text;
    public StyleId<RunLayout> Style { get; init; }
}