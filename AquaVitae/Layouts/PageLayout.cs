using AquaVitae.Common;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class PageLayout(PageSize pageSize) : ILayout
{
    public PageSize PageSize => pageSize;
    public Color? FillColor { get; init; }
    private OptionalList<IPageElementLayout> _pageElements;
    public IReadOnlyList<IPageElementLayout> PageElements => _pageElements.AsReadOnly();
    
    public void AddPageElement(IPageElementLayout pageElement) => _pageElements.Add(pageElement);
}