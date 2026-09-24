using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public closed class ElementLayoutBase : LayoutBase
{
    public virtual PrintPoint X { get; }
    public virtual PrintPoint Y { get; }
}