using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Styles;

public sealed record RunStyle(string Name, string Id, StyleId<FontStyle> Font) : IStyle<RunLayout>
{
    public PrintPoint FontSize { get; init; } = 11;
    public Color Color { get; init; } = new(0, 0, 0);
    public bool Bold { get; init; }
    public bool Italic { get; init; }
    public bool Underline { get; init; }
    public bool Strikethrough { get; init; }
}