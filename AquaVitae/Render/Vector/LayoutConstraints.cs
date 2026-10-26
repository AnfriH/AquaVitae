using AquaVitae.Layouts.Types;

namespace AquaVitae.Render.Vector;

/// <summary>
/// Represents the constraints used to render a layout
/// </summary>
public readonly record struct LayoutConstraints(PrintPoint X, PrintPoint Y, PrintPoint Width, PrintPoint Height);