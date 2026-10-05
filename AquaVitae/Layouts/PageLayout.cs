using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class PageLayout(PageSize pageSize) : LayoutBase
{
    public PageSize PageSize => pageSize;
    public PageOverflowBehaviour OverflowBehaviour { get; init; } = PageOverflowBehaviour.Truncate;
    public Color PageColor { get; init; } = Colors.White;
    public CanvasLayout? Background { get; init; }
    public GridLayout? Grid { get; init; }
    public CanvasLayout? Foreground { get; init; }
}

public enum PageOverflowBehaviour
{
    /// <summary>
    /// Overflowing content will be truncated.
    /// </summary>
    Truncate,
    
    /// <summary>
    /// Overflowing content will continue onto the next page but may be segmented across multiple pages.
    /// </summary>
    Continuous,
    
    /// <summary>
    /// Overflowing content will continue onto the next page. Unlike <see cref="Continuous"/>,
    /// the layout engine will try to avoid splitting elements across multiple pages.
    /// Of course, segmentation may still occur if:
    /// <list type="bullet">
    /// <item>The element is too large to fit on a single page.</item>
    /// <item>The element is placed on the <see cref="PageLayout.Foreground">Foreground</see> layer.</item>
    /// <item>The <see cref="PageLayout.Grid">Grid</see> layer does not have a continuous horizontal break</item>
    /// </list>
    /// </summary>
    PageFit,
    
    /// <summary>
    /// Overflowing content will result in the page being lengthened to fit the content. This may cause
    /// printouts to look weird.
    /// </summary>
    Scale
}