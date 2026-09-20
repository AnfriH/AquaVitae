using AquaVitae.Common;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class PageLayout(PageSize pageSize) : ILayout
{
    private OptionalList<IPageElementLayout> _pageElements;
    public IReadOnlyList<IPageElementLayout> PageElements => _pageElements.AsReadOnly();
    public PageSize PageSize => pageSize;
    public Color? FillColor { get; init; }
    public void AddPageElement(IPageElementLayout pageElement) => _pageElements.Add(pageElement);
}