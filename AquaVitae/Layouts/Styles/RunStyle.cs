using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Styles;

public sealed record RunStyle(string Name, string Id) : IStyle<RunLayout>
{
    public string? FontName { get; init; }
    public PrintPoint? FontSize { get; init; }
    public Color? Color { get; init; }
    public bool Bold { get; init; }
    public bool Italic { get; init; }
    public bool Underline { get; init; }
    public bool Strikethrough { get; init; }
}