using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Grids;

/// <summary>
/// Represents a grid element after compilation, where the relative positions and widths have been calculated.
/// </summary>
/// <param name="XFraction">The relative X offset (0 to 1) of this element within the grid.</param>
/// <param name="WidthFraction">The relative width (0 to 1) of this element within the grid.</param>
/// <param name="MinimumHeight">The minimum height of this element within the grid. Any remaining space will be padded.</param>
/// <param name="MaximumHeight">The maximum height of this element within the grid. Any overflow will be truncated.</param>
/// <param name="ChildrenIndex">The lookup index to get the children within the parent <see cref="CompiledGridLayout"/></param>
/// <param name="Element">The layout this element will render.</param>
public sealed record CompiledGridElement(
    float XFraction,
    float WidthFraction,
    PrintPoint MinimumHeight,
    PrintPoint MaximumHeight,
    int ChildrenIndex,
    ElementLayoutBase Element
);