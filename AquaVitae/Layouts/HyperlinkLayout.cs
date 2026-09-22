namespace AquaVitae.Layouts;

public class HyperlinkLayout(Uri uri, string? text = null) : RunLayoutBase(text ?? uri.ToString())
{
    public Uri Uri => uri;
}