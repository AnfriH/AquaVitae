namespace AquaVitae.Layouts.Styles;

public sealed record ParagraphStyle(string Name, string Id) : IStyle<ParagraphLayout>
{
    public StyleId<RunLayoutBase> RunId { get; init; }
    public ParagraphAlignment Alignment { get; init; } = ParagraphAlignment.Left;
}

public enum ParagraphAlignment
{
    Left,
    Right,
    Center,
    Justify
}