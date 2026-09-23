using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public closed class PageElementLayoutBase : LayoutBase
{
    public virtual PrintPoint X { get; }
    public virtual PrintPoint Y { get; }
}