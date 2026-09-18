using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class PageLayout(PageSize pageSize) : ILayout
{
    public PageSize PageSize => pageSize;
    
    private readonly List<IPageElementLayout> _pageElements = [];
    public IReadOnlyList<IPageElementLayout> PageElements => _pageElements;
    
    public void AddPageElement(IPageElementLayout pageElement) => _pageElements.Add(pageElement);
}