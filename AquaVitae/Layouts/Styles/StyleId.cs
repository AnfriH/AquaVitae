namespace AquaVitae.Layouts.Styles;

public record struct StyleId<TLayout>(string? Value) where TLayout : ILayout
{
    public static implicit operator string?(StyleId<TLayout> layout)
    {
        return layout.Value;
    }
}