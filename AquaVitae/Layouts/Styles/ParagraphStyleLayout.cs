namespace AquaVitae.Layouts.Styles;

public sealed class ParagraphStyleLayout(string name, string id) : IStyleLayout
{
    public string Name => name;
    public string Id => id;
    public string? RunStyleId { get; init; }
    public ParagraphAlignment Alignment { get; init; } = ParagraphAlignment.Left;
}

public enum ParagraphAlignment
{
    Left,
    Right,
    Center,
    Justify
}