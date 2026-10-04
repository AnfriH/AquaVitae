using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts.Abstractions;

public closed class ParagraphLayoutBase(StyleId<ParagraphStyle> style) : LayoutBase, IComparable<ParagraphLayoutBase>
{
    // Higher priority paragraphs are inserted earlier into the docx body. Priority is from smallest to largest.
    public int Priority { get; init; }
    public StyleId<ParagraphStyle> Style => style;

    public int CompareTo(ParagraphLayoutBase? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        return other is null ? 1 : Priority.CompareTo(other.Priority);
    }
}