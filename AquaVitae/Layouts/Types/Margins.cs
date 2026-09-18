namespace AquaVitae.Layouts.Types;

public sealed record Margins(Length Top, Length Bottom, Length Left, Length Right)
{
    public static readonly Margins Zero = new(0, 0, 0, 0);
}