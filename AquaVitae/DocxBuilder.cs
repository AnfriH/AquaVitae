using AquaVitae.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae;

public sealed class DocxBuilder
{
    private DocxBuilder()
    {
    }

    public static void Build(CvData data, Stream outStream)
    {
        using var document = WordprocessingDocument.Create(outStream, WordprocessingDocumentType.Document);
        
        new DocxBuilder().BuildDocument(data, document);
    }

    private void BuildDocument(CvData data, WordprocessingDocument document)
    {
        var mainPart = document.AddMainDocumentPart();

        mainPart.Document = new Document();
        var body = mainPart.Document.AppendChild(new Body());
        
        BuildDocumentBody(data, body);
        
        mainPart.Document.Save();
    }

    private void BuildDocumentBody(CvData data, Body body)
    {
        BuildSideSection(data, body);
        BuildMainSection(data, body);
    }

    private void BuildSideSection(CvData data, Body body)
    {
        BuildTitle(data.Profile, body);
        BuildContactInfo(data.Profile, body);
        
        BuildSoftSkills(data.Skills.Soft, body);
        BuildTechnicalSkills(data.Skills.Technical, body);
        
        BuildLanguages(data.ProgrammingLanguages, body);
    }

    private void BuildTitle(ProfileData profile, Body body)
    {
        body.AddParagraph().AddRun(profile.Name);
        body.AddParagraph().AddRun(profile.Role);
    }

    private void BuildContactInfo(ProfileData profile, Body body)
    {
        body.AddParagraph().AddRun("Contact");
        
        body.AddParagraph().AddRun("Address");
        body.AddParagraph().AddRun(profile.Address);

        body.AddParagraph().AddRun("Phone");
        body.AddParagraph().AddRun(profile.Phone);
        
        body.AddParagraph().AddRun("Email");
        body.AddParagraph().AddRun(profile.Email);
        
        body.AddParagraph().AddRun("LinkedIn");
        body.AddParagraph().AddRun(profile.LinkedIn);
        
        body.AddParagraph().AddRun("Github");
        body.AddParagraph().AddRun(profile.GitHub);
        
        // body.AddParagraph().AddRun("Website");
        // body.AddParagraph().AddRun(profile.Website);
    }

    private void BuildSoftSkills(string[] skills, Body body)
    {
        body.AddParagraph().AddRun("Soft Skills");
        foreach (var skill in skills)
        {
            body.AddParagraph().AddRun(skill);
        }
    }
    
    private void BuildTechnicalSkills(string[] skills, Body body)
    {
        body.AddParagraph().AddRun("Technical Skills");
        foreach (var skill in skills)
        {
            body.AddParagraph().AddRun(skill);
        }
    }
    
    private void BuildLanguages(ProgrammingLanguageData[] languages, Body body)
    {
        body.AddParagraph().AddRun("Programming Languages");

        foreach (var language in languages)
        {
            body.AddParagraph().AddRun(language.Name);
            body.AddParagraph().AddRun(language.SkillLevel.ToText());
        }
    }

    private void BuildMainSection(CvData data, Body body)
    {
        BuildBlurb(data.Profile.Blurb, body);
        BuildWorkHistory(data.Experience, body);
        BuildEducationHistory(data.Education, body);
    }

    private void BuildBlurb(string blurb, Body body)
    {
        body.AddParagraph().AddRun(blurb);
    }

    private void BuildWorkHistory(ExperienceData[] experience, Body body)
    {
        body.AddParagraph().AddRun("Work History");
        foreach (var job in experience)
        {
            BuildWorkEntry(job, body);
        }
    }

    private void BuildWorkEntry(ExperienceData job, Body body)
    {
        var startDate = job.GetStartDate().ToString("MM-yyyy");
        var endDate = job.GetEndDate()?.ToString("MM-yyyy") ?? "Current";

        body.AddParagraph().AddRun($"{startDate} - {endDate}");

        body.AddParagraph().AddRun(job.Role);
        body.AddParagraph().AddRun($"{job.Company} - {job.Location}");

        foreach (var responsibility in job.Responsibilities)
        {
            // TODO: Impl as bullet points
            body.AddParagraph().AddRun($"- {responsibility}");
        }
    }

    private void BuildEducationHistory(EducationData[] education, Body body)
    {
        body.AddParagraph().AddRun("Education");
        foreach (var qualification in education)
        {
            BuildQualificationEntry(qualification, body);
        }
    }

    private void BuildQualificationEntry(EducationData qualification, Body body)
    {
        body.AddParagraph().AddRun(qualification.GetCompletedDate().ToString("MM-yyyy"));

        body.AddParagraph().AddRun(qualification.Qualification);
        body.AddParagraph().AddRun($"{qualification.Institution} - {qualification.Location}");
    }
}

public static class DocxExtensions
{
    public static Paragraph AddParagraph(this Body body)
    {
        return body.AppendChild(new Paragraph());
    }

    public static Run AddRun(this Paragraph paragraph, string text)
    {
        var run = paragraph.AppendChild(new Run());
        run.AppendChild(new Text(text));
        return run;
    }
}