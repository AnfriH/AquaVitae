using Tomlyn.Serialization;

namespace AquaVitae.Data;

public record ProgrammingLanguageData(
    string Name,
    [property: TomlPropertyName("level")] SkillLevelData SkillLevel
);

public enum SkillLevelData
{
    Beginner = 1,
    Novice = 2,
    Intermediate = 3,
    Expert = 4,
    Professional = 5
}

public static class SkillLevelDataExtensions
{
    public static string ToText(this SkillLevelData level)
    {
        return level switch
        {
            SkillLevelData.Beginner => "Beginner",
            SkillLevelData.Novice => "Novice",
            SkillLevelData.Intermediate => "Intermediate",
            SkillLevelData.Expert => "Expert",
            SkillLevelData.Professional => "Professional",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }
}