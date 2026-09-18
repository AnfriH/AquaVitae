namespace AquaVitae.Layouts;

public sealed class DocumentLayout : ILayout
{
    private readonly List<PageLayout> _pages = [];
    public IReadOnlyList<PageLayout> Pages => _pages;
    
    public void AddPage(PageLayout page) => _pages.Add(page);
}