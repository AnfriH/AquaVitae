using AquaVitae.Common;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class DocumentLayout : ILayout
{
    private OptionalList<PageLayout> _pages;
    public IReadOnlyList<PageLayout> Pages => _pages.AsReadOnly();
    
    private OptionalDictionary<string, RunStyle> _runStyles;
    private OptionalDictionary<string, ParagraphStyle> _paragraphStyles;
    public IReadOnlyDictionary<string, RunStyle> RunStyles => _runStyles.AsReadOnly();
    public IReadOnlyDictionary<string, ParagraphStyle> ParagraphStyles => _paragraphStyles.AsReadOnly();
    
    public void AddPage(PageLayout page) => _pages.Add(page);

    public StyleId<RunLayout> AddStyle(RunStyle style)
    {
        _runStyles[style.Id] = style;
        return new StyleId<RunLayout>(style.Id);
    }

    public StyleId<ParagraphLayout> AddStyle(ParagraphStyle style)
    {
        _paragraphStyles[style.Id] = style;
        return new StyleId<ParagraphLayout>(style.Id);
    }
}