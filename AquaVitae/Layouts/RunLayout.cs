using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class RunLayout(string text) : ILayout
{
    public string Text => text;
    public PrintPoint FontSize { get; init; } = new(12);
    public bool Bold { get; init; } = false;
    public bool Italic { get; init; } = false;
    public bool Underline { get; init; } = false;
    public bool Strikethrough { get; init; } = false;
    public Color? TextColor { get; init; } = null;
}