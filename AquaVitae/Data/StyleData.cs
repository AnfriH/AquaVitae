using AquaVitae.Layouts.Types;
using Tomlyn.Serialization;

namespace AquaVitae.Data;

public record StyleData(
    string Primary,
    string Secondary,
    [property: TomlPropertyName("font_name")] string FontName,
    [property: TomlPropertyName("font_path_regular")] string FontPathRegular,
    [property: TomlPropertyName("font_path_bold")] string FontPathBold
)
{
    [TomlIgnore] public Color PrimaryColor { get; } = Color.FromString(Primary);
    [TomlIgnore] public Color SecondaryColor { get; } = Color.FromString(Secondary);
}