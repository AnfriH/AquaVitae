namespace AquaVitae.Layouts.Styles.Lists;

public sealed class NumberedMarkerStyle(string suffix) : IListMarkerStyle
{
    public bool Ordered => true;
    public string GetStyle(int level)
    {
        return '%'+level+suffix;
    }
}