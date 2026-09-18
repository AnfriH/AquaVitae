namespace AquaVitae.Layouts.Types;

public sealed record Margins(PrintPoint Top, PrintPoint Bottom, PrintPoint Left, PrintPoint Right)
{
    public static readonly Margins Zero = new(0, 0, 0, 0);
}