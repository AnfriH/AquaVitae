using AquaVitae.Common;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class DocumentLayout : LayoutBase
{
    private OptionalList<PageLayout> _pages;
    public IReadOnlyList<PageLayout> Pages => _pages.AsReadOnly();
    
    private OptionalDictionary<string, RunStyle> _runStyles;
    public IReadOnlyDictionary<string, RunStyle> RunStyles => _runStyles.AsReadOnly();
    
    private OptionalDictionary<string, ParagraphStyle> _paragraphStyles;
    public IReadOnlyDictionary<string, ParagraphStyle> ParagraphStyles => _paragraphStyles.AsReadOnly();
    
    private OptionalDictionary<string, VerticalListStyle> _verticalListStyles;
    public IReadOnlyDictionary<string, VerticalListStyle> VerticalListStyles => _verticalListStyles.AsReadOnly();
    
    public void AddPage(PageLayout page) => _pages.Add(page);

    public StyleId<RunLayout> AddRunStyle(RunStyle style)
    {
        _runStyles[style.Id] = style;
        return new StyleId<RunLayout>(style.Id);
    }

    public StyleId<ParagraphLayout> AddParagraphStyle(ParagraphStyle style)
    {
        _paragraphStyles[style.Id] = style;
        return new StyleId<ParagraphLayout>(style.Id);
    }

    public StyleId<VerticalListLayout> AddVerticalListStyle(VerticalListStyle style)
    {
        _verticalListStyles[style.Id] = style;
        return new StyleId<VerticalListLayout>(style.Id);
    }
}