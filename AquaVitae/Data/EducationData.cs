using System.Globalization;
using Tomlyn.Serialization;

namespace AquaVitae.Data;

public record EducationData(
    string Qualification,
    string Institution,
    string Location,
    [property: TomlPropertyName("completed")] string CompletedRaw
)
{
    public DateTime GetCompletedDate()
    {
        return DateTime.ParseExact(CompletedRaw, "MM/yyyy", CultureInfo.InvariantCulture);
    }
}