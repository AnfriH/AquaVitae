using AquaVitae.Common;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class DocumentLayout : ILayout
{
    private OptionalList<PageLayout> _pages;
    public IReadOnlyList<PageLayout> Pages => _pages.AsReadOnly();
    
    private OptionalDictionary<string, RunStyleLayout> _runStyles;
    private OptionalDictionary<string, ParagraphStyleLayout> _paragraphStyles;
    public IReadOnlyDictionary<string, RunStyleLayout> RunStyles => _runStyles.AsReadOnly();
    public IReadOnlyDictionary<string, ParagraphStyleLayout> ParagraphStyles => _paragraphStyles.AsReadOnly();
    
    public void AddPage(PageLayout page) => _pages.Add(page);
    public void AddStyle(RunStyleLayout style) => _runStyles[style.Id] = style;
    public void AddStyle(ParagraphStyleLayout style) => _paragraphStyles[style.Id] = style;
}