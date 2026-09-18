using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class BoxLayout(Length x, Length y, Length width, Length height) : IPageElementLayout
{
    public Length X => x;
    public Length Y => y;
    public Length Width => width;
    public Length Height => height;
    
    private readonly List<ParagraphLayout> _paragraphs = [];
    public IReadOnlyList<ParagraphLayout> Paragraphs => _paragraphs;
    
    public Color? FillColor { get; set; }
    public Margins InnerMargins { get; set; } = Margins.Zero;
    public Margins OuterMargins { get; set; } = Margins.Zero;
    
    public void AddParagraph(ParagraphLayout paragraph) => _paragraphs.Add(paragraph);
}