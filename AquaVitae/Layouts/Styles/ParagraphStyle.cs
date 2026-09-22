using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Styles;

public sealed record ParagraphStyle(string Name, string Id) : IStyle<ParagraphLayout>
{
    public StyleId<RunLayoutBase> RunId { get; init; }
    public ParagraphAlignment Alignment { get; init; } = ParagraphAlignment.Left;
    public PrintPoint Before { get; init; }
    public PrintPoint After { get; init; }
}

public enum ParagraphAlignment
{
    Left,
    Right,
    Center,
    Justify
}