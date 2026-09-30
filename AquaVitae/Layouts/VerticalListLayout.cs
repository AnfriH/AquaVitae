using AquaVitae.Common;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class VerticalListLayout(
    StyleId<ParagraphStyle> style,
    StyleId<VerticalListStyle> listStyle
) : ParagraphLayoutBase(style)
{
    public StyleId<VerticalListStyle> ListStyle => listStyle;
    private OptionalList<ParagraphLayoutBase> _paragraphs;
    public IReadOnlyList<ParagraphLayoutBase> Paragraphs => _paragraphs.AsReadOnly();
    public void AddParagraph(ParagraphLayoutBase paragraph) => _paragraphs.Add(paragraph);
}