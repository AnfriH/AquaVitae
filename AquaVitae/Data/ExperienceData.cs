using System.Globalization;
using Tomlyn.Serialization;

namespace AquaVitae.Data;

public record ExperienceData(
    [property: TomlPropertyName("start")] string StartRaw,
    [property: TomlPropertyName("end")] string EndRaw,
    string Role,
    string Company,
    string Location,
    string[] Responsibilities
)
{
    public DateTime GetStartDate() => DateTime.ParseExact(StartRaw, "MM/yyyy", CultureInfo.InvariantCulture);
    public DateTime? GetEndDate()
    {
        if (string.IsNullOrEmpty(EndRaw)) return null;
        return DateTime.ParseExact(EndRaw, "MM/yyyy", CultureInfo.InvariantCulture);
    }
}