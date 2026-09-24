using AquaVitae.Common;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class PageLayout(PageSize pageSize) : LayoutBase
{
    private OptionalList<ElementLayoutBase> _pageElements;
    public IReadOnlyList<ElementLayoutBase> PageElements => _pageElements.AsReadOnly();
    public PageSize PageSize => pageSize;
    public Color? FillColor { get; init; }
    public void AddPageElement(ElementLayoutBase element) => _pageElements.Add(element);
}