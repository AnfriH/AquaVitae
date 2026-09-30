using AquaVitae.Common;

namespace AquaVitae.Layouts;

public sealed class DocumentLayout : LayoutBase
{
    private OptionalList<PageLayout> _pages;
    public IReadOnlyList<PageLayout> Pages => _pages.AsReadOnly();
    public StyleLayout Styles { get; } = new();

    public void AddPage(PageLayout page)
    {
        _pages.Add(page);
    }
}