using AquaVitae.Common;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class VerticalListLayout(StyleId<VerticalListLayout> style) : ParagraphLayoutBase
{
    private OptionalList<ParagraphLayoutBase> _paragraphs;
    public IReadOnlyList<ParagraphLayoutBase> Paragraphs => _paragraphs.AsReadOnly();
    public StyleId<VerticalListLayout> Style => style;
    public void AddParagraph(ParagraphLayoutBase paragraph) => _paragraphs.Add(paragraph);
}