using AquaVitae.Common;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class BoxLayout(PrintPoint x, PrintPoint y, PrintPoint width, PrintPoint height) : IPageElementLayout
{
    public PrintPoint X => x;
    public PrintPoint Y => y;
    public PrintPoint Width => width;
    public PrintPoint Height => height;
    
    private OptionalList<ParagraphLayout> _paragraphs;
    public IReadOnlyList<ParagraphLayout> Paragraphs => _paragraphs.AsReadOnly();
    
    public Color? FillColor { get; init; }
    public Margins InnerMargins { get; init; } = Margins.Zero;
    public Margins OuterMargins { get; init; } = Margins.Zero;
    
    public void AddParagraph(ParagraphLayout paragraph) => _paragraphs.Add(paragraph);
}