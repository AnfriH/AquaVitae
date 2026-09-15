using System.Text.Json.Serialization;
using Tomlyn.Serialization;

namespace AquaVitae.Data;

[TomlSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
)]
[TomlSerializable(typeof(CvData))]
public partial class CvDataContext : TomlSerializerContext
{
}