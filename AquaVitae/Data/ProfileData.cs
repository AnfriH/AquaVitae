using Tomlyn.Serialization;

namespace AquaVitae.Data;

public record ProfileData(
    string Name,
    string Role,
    string Email,
    string Phone,
    string Address,
    [property: TomlPropertyName("linkedin")] string LinkedIn,
    [property: TomlPropertyName("github")] string GitHub,
    string Blurb
);