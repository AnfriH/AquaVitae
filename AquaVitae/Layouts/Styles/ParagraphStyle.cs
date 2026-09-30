using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Styles;

public sealed record ParagraphStyle(string Name, string Id) : IStyle<ParagraphLayout>
{
    public PrintPoint? Before { get; init; }
    public PrintPoint? After { get; init; }
    public PrintPoint? Between { get; init; }
}