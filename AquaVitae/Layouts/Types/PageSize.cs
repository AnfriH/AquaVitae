namespace AquaVitae.Layouts.Types;

public readonly record struct PageSize(Length Width, Length Height);

public static class PageSizes
{
    public static readonly PageSize A4 = new(Length.FromMillimeters(210), Length.FromMillimeters(297));
    public static readonly PageSize USLetter = new(Length.FromInches(8.5f), Length.FromInches(11));
}