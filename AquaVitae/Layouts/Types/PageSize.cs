namespace AquaVitae.Layouts.Types;

public readonly record struct PageSize(PrintPoint Width, PrintPoint Height);

public static class PageSizes
{
    public static readonly PageSize A4 = new(PrintPoint.FromMillimeters(210), PrintPoint.FromMillimeters(297));
    public static readonly PageSize USLetter = new(PrintPoint.FromInches(8.5f), PrintPoint.FromInches(11));
}