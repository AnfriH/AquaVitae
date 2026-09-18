namespace AquaVitae.Layouts;

public sealed class RunLayout(string text) : ILayout
{
    public string Text => text;
}