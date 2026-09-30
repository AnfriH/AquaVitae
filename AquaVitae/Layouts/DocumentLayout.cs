using AquaVitae.Common;
using AquaVitae.Layouts.Styles;
using AquaVitae.Layouts.Styles.Lists;

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