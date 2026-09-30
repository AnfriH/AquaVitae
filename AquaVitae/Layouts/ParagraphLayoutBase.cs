using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public closed class ParagraphLayoutBase(StyleId<ParagraphStyle> style) : LayoutBase
{
    public StyleId<ParagraphStyle> Style => style;
}