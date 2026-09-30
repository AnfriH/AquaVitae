using AquaVitae.Common;
using AquaVitae.Layouts.Styles;
using AquaVitae.Layouts.Styles.Lists;

namespace AquaVitae.Layouts;

public sealed class VerticalListLayout(StyleId<VerticalListStyle> style) : ParagraphLayoutBase
{
    private OptionalList<ParagraphLayoutBase> _paragraphs;
    public IReadOnlyList<ParagraphLayoutBase> Paragraphs => _paragraphs.AsReadOnly();
    public StyleId<VerticalListStyle> Style => style;
    public void AddParagraph(ParagraphLayoutBase paragraph) => _paragraphs.Add(paragraph);
}