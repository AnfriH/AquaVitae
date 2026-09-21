using AquaVitae.Common;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class PageLayout(PageSize pageSize) : LayoutBase
{
    private OptionalList<PageElementLayoutBase> _pageElements;
    public IReadOnlyList<PageElementLayoutBase> PageElements => _pageElements.AsReadOnly();
    public PageSize PageSize => pageSize;
    public Color? FillColor { get; init; }
    public void AddPageElement(PageElementLayoutBase pageElement) => _pageElements.Add(pageElement);
}