using AquaVitae.Common;

namespace AquaVitae.Layouts;

public sealed class DocumentLayout : ILayout
{
    private OptionalList<PageLayout> _pages;
    public IReadOnlyList<PageLayout> Pages => _pages.AsReadOnly();
    public void AddPage(PageLayout page) => _pages.Add(page);
}