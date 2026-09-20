using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Styles;

public sealed class RunStyle(string name, string id) : IStyle<RunLayout>
{
    public string Name => name;
    public string Id => new(id);
    public string? Font { get; init; }
    public PrintPoint? FontSize { get; init; }
    public Color? Color { get; init; }
    public bool Bold { get; init; }
    public bool Italic { get; init; }
    public bool Underline { get; init; }
    public bool Strikethrough { get; init; }
}