using Tomlyn.Serialization;

namespace AquaVitae.Data;

public record CvData(
    StyleData Style,
    ProfileData Profile,
    SkillsData Skills,
    ExperienceData[] Experience,
    EducationData[] Education,
    [property: TomlPropertyName("languages") ] ProgrammingLanguageData[] ProgrammingLanguages
);