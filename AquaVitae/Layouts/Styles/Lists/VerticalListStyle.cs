using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Styles.Lists;

public sealed record VerticalListStyle(string Name, string Id, PrintPoint Indentation) : IStyle<VerticalListLayout>
{
    /// <summary>
    /// The style of the list marker. Some convenient defaults are defined in <see cref="ListMarkerStyles"/>.
    /// </summary>
    public IListMarkerStyle MarkerStyle { get; init; } = ListMarkerStyles.Bullet;
}