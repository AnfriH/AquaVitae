using AquaVitae.Common;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class BoxLayout(PrintPoint x, PrintPoint y, PrintPoint width, PrintPoint height) : PageElementLayoutBase
{
    public PrintPoint X => x;
    public PrintPoint Y => y;
    public PrintPoint Width => width;
    public PrintPoint Height => height;
    
    private OptionalList<ParagraphLayoutBase> _paragraphs;
    public IReadOnlyList<ParagraphLayoutBase> Paragraphs => _paragraphs.AsReadOnly();
    
    public Color? FillColor { get; init; }
    public Margins InnerMargins { get; init; } = Margins.Zero;
    public Margins OuterMargins { get; init; } = Margins.Zero;
    
    public void AddParagraph(ParagraphLayoutBase paragraph) => _paragraphs.Add(paragraph);
}