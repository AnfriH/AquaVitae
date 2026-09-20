namespace AquaVitae.Layouts.Styles;

public sealed class ParagraphStyle(string name, string id) : IStyle<ParagraphLayout>
{
    public string Name => name;
    public string Id => new(id);
    public StyleId<RunLayout> RunId { get; init; }
    public ParagraphAlignment Alignment { get; init; } = ParagraphAlignment.Left;
}

public enum ParagraphAlignment
{
    Left,
    Right,
    Center,
    Justify
}