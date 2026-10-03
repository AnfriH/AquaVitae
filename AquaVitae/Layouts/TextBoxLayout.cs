using AquaVitae.Common;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class TextBoxLayout : ElementLayoutBase
{
    private OptionalList<ParagraphLayoutBase> _paragraphs;
    public IReadOnlyList<ParagraphLayoutBase> Paragraphs => _paragraphs.AsReadOnly();
    
    public Color? FillColor { get; init; }
    public Margins InnerMargins { get; init; } = Margins.Zero;
    public PrintPoint ParagraphSpacing { get; init; } = 0;
    public void AddParagraph(ParagraphLayoutBase paragraph) => _paragraphs.Add(paragraph);
    
    public override void CollectParagraphs(Action<ParagraphLayoutBase> callback)
    {
        foreach (var paragraph in _paragraphs)
        {
            callback(paragraph);
        }
    }
}