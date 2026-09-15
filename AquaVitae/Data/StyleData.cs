using Tomlyn.Serialization;

namespace AquaVitae.Data;

public record StyleData(
    [property: TomlPropertyName("primary")] string PrimaryColour,
    [property: TomlPropertyName("secondary")] string SecondaryColour,
    [property: TomlPropertyName("text")] string TextColour
);