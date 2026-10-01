using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Styles;

public sealed record ParagraphStyle(string Name, string Id) : IStyle<ParagraphLayout>
{
    public StyleId<RunStyle> RunStyle { get; init; }
    public PrintPoint LineSpacing { get; init; } = 0;
}