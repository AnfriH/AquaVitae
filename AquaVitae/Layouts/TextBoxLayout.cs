using AquaVitae.Common;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class TextBoxLayout(PrintPoint x, PrintPoint y, PrintPoint width, PrintPoint height) : ElementLayoutBase
{
    public override PrintPoint X => x;
    public override PrintPoint Y => y;
    public PrintPoint Width => width;
    public PrintPoint Height => height;
    
    private OptionalList<ParagraphLayoutBase> _paragraphs;
    public IReadOnlyList<ParagraphLayoutBase> Paragraphs => _paragraphs.AsReadOnly();
    
    public Color? FillColor { get; init; }
    public Margins InnerMargins { get; init; } = Margins.Zero;
    
    public void AddParagraph(ParagraphLayoutBase paragraph) => _paragraphs.Add(paragraph);
}