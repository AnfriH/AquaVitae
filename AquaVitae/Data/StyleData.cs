using AquaVitae.Layouts.Types;

namespace AquaVitae.Data;

public class StyleData(
    string primary,
    string secondary
)
{
    public Color PrimaryColor { get; } = Color.FromString(primary);
    public Color SecondaryColor { get; } = Color.FromString(secondary);
}