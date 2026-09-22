namespace AquaVitae.Layouts.Styles.Lists;

public sealed class UnorderedMarkerStyle(string point) : IListMarkerStyle
{
    public bool Ordered => false;
    public string GetStyle(int level)
    {
        return point;
    }
}