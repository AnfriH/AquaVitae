using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class PageLayout(PageSize pageSize, Color? pageColor = null) : LayoutBase
{
    public PageSize PageSize => pageSize;
    public Color PageColor { get; } = pageColor ?? Colors.White;

    private CanvasLayout? _background;
    public CanvasLayout Background => _background ??= new CanvasLayout();
    
    private GridLayout? _grid;
    public GridLayout Grid => _grid ??= new GridLayout();

    private CanvasLayout? _foreground;
    public CanvasLayout Foreground => _foreground ??= new CanvasLayout();
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
    PageFit
}