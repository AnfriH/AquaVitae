using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Styles;

public sealed record VerticalListStyle(string Name, string Id) : IStyle<VerticalListLayout>
{
    public Color ElementColor { get; init; } = new(0, 0, 0);
    public PrintPoint Indent { get; init; }
    public bool Ordered { get; init; }
}