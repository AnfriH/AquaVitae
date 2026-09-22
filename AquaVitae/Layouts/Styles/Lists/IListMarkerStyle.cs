namespace AquaVitae.Layouts.Styles.Lists;

public interface IListMarkerStyle
{
    public bool Ordered { get; }
    public string GetStyle(int level);
}