using System.Text;
using AquaVitae.Data;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Grids;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Outputs;

public static class CvColors
{
    public static readonly Color White = new(0xFFFFFFFFU);
    public static readonly Color Grey = new(0xFF343434U);
}

public sealed class CvBuilder
{
    private readonly CvData _data;
    private readonly DocumentLayout _documentLayout;
    private readonly CvStylesCollection _styles;
    
    private static readonly Margins DefaultMargins = new(12, 12, 18, 18);
    private static readonly Margins NameMargins = new(-6, 12, 18, 18);
    private static readonly Margins BlurbMargins = new(0, 12, 18, 18);
    private static readonly Margins LeftHeadingMargins = new(3, -6, 18, 18);
    private static readonly Margins RightBulletMargins = new(3, 0, 18, 18);
    private static readonly Margins PageMargins = new(18, 18, 0, 0);
    
    private static readonly PrintPoint LeftWidth = PageSizes.A4.Width / 3;
    
    public CvBuilder(CvData data)
    {
        _data = data;
        _documentLayout = new DocumentLayout();
        _styles = CvStylesCollection.Build(_documentLayout.Styles, data.Style);
    }
    
    public DocumentLayout Build()
    {
        _documentLayout.AddPage(BuildFirstPage());
        // _documentLayout.AddPage(BuildSecondPage());

        return _documentLayout;
    }

    private PageLayout BuildFirstPage()
    {
        // divided into 1/3 and 2/3 sections
        var gridBuilder = new TableBuilder(1, 2);

        var leftBuilder = new TableBuilder(1);
        var rightBuilder = new TableBuilder(1);
        
        gridBuilder.AddCell(new SubgridCell(leftBuilder, 0, 0));
        gridBuilder.AddCell(new SubgridCell(rightBuilder, 0, 1));
        
        // Left side
        leftBuilder
            .AddCell(new ElementCell(BuildTitle(), 0, 0))
            .AddCell(new ElementCell(BuildLeftHeading("Contact", 0), 1, 0))
            .AddCell(new ElementCell(BuildLeftContact(), 2, 0))
            .AddCell(new ElementCell(BuildLeftSkills("Technical Skills", _data.Skills.Technical), 3, 0))
            .AddCell(new ElementCell(BuildLeftSkills("Soft Skills", _data.Skills.Soft), 4, 0))
            .AddCell(new ElementCell(BuildLeftLanguages(), 5, 0));
        
        // Right side
        var i = 0;
        rightBuilder
            .AddCell(new ElementCell(BuildRightBlurb(), i++, 0))
            .AddCell(new ElementCell(BuildRightHeading("Work History"), i++, 0));
        
        foreach (var experience in _data.Experience)
        {
            rightBuilder.AddCell(new ElementCell(BuildRightExperience(experience), i++, 0));
        }

        rightBuilder.AddCell(new ElementCell(new BoxLayout { Height = 12 }, i++, 0));
        rightBuilder.AddCell(new ElementCell(BuildRightHeading("Education"), i++, 0));

        foreach (var education in _data.Education)
        {
            rightBuilder.AddCell(new ElementCell(BuildRightEducation(education), i++, 0));
        }
        
        var pageLayout = CreatePageTemplate(gridBuilder.Compile());
        return pageLayout;
    }

    private CompiledGridLayout BuildRightExperience(ExperienceData experience)
    {
        var experienceGrid = new TableBuilder(1, 3);

        var titleTextbox = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        var titlePara = new ParagraphLayout(_styles.Paragraph2) { Priority = 3 };
        titlePara.AddRun(new RunLayout(experience.Role, _styles.RightHeading));
        titleTextbox.AddParagraph(titlePara);

        experienceGrid.AddCell(new ElementCell(titleTextbox, 0, 1));
        
        var datesLayout = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        
        var datesPara = new ParagraphLayout(_styles.Paragraph2) { Priority = 3 };
        datesPara.AddRun(new RunLayout($"{experience.GetStartDate():yyyy-MM} - {experience.GetEndDate()?.ToString("yyyy-MM") ?? "current"}", _styles.RightParagraph));
        
        datesLayout.AddParagraph(datesPara);
        
        experienceGrid.AddCell(new ElementCell(datesLayout, 0, 0) { RowSpan = 2 });

        var roleLayout = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        var rolePara = new ParagraphLayout(_styles.Paragraph2) { Priority = 3 };
        roleLayout.AddParagraph(rolePara);
        
        rolePara.AddRun(new RunLayout(experience.Company, _styles.RightParagraph));
        experienceGrid.AddCell(new ElementCell(roleLayout, 1, 1));
        
        var respLayout = new TextBoxLayout
        {
            InnerMargins = RightBulletMargins
        };
        
        var respPara = new VerticalListLayout(_styles.Paragraph2, _styles.RightList) { Priority = 3 };
        respLayout.AddParagraph(respPara);

        foreach (var resp in experience.Responsibilities)
        {
            var itemPara = new ParagraphLayout(_styles.Paragraph2);
            itemPara.AddRun(new RunLayout(resp, _styles.RightParagraph));
            respPara.AddParagraph(itemPara);
        }
        experienceGrid.AddCell(new ElementCell(respLayout, 2, 1));

        return experienceGrid.Compile();
    }
    
    private CompiledGridLayout BuildRightEducation(EducationData educationData)
    {
        var experienceGrid = new TableBuilder(1, 3);

        var titleTextbox = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        var titlePara = new ParagraphLayout(_styles.Paragraph2) { Priority = 3 };
        titlePara.AddRun(new RunLayout(educationData.Qualification, _styles.RightHeading));
        titleTextbox.AddParagraph(titlePara);

        experienceGrid.AddCell(new ElementCell(titleTextbox, 0, 1));
        
        var datesLayout = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        
        var datesPara = new ParagraphLayout(_styles.Paragraph2) { Priority = 3 };
        datesPara.AddRun(new RunLayout($"{educationData.GetCompletedDate():yyyy-MM}", _styles.RightParagraph));
        
        datesLayout.AddParagraph(datesPara);
        
        experienceGrid.AddCell(new ElementCell(datesLayout, 0, 0) { RowSpan = 2 });

        var roleLayout = new TextBoxLayout
        {
            InnerMargins = RightBulletMargins
        };
        var rolePara = new ParagraphLayout(_styles.Paragraph2) { Priority = 3 };
        roleLayout.AddParagraph(rolePara);
        
        rolePara.AddRun(new RunLayout(educationData.Institution, _styles.RightParagraph));
        experienceGrid.AddCell(new ElementCell(roleLayout, 1, 1));

        return experienceGrid.Compile();
    }

    private CompiledGridLayout BuildRightHeading(string heading)
    {
        var headingGrid = new TableBuilder(1);

        headingGrid.AddCell(new ElementCell(new BoxLayout
        {
            Height = 0.25f,
            FillColor = CvColors.Grey
        }, 0, 0));

        var textBox = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        headingGrid.AddCell(new ElementCell(textBox, 1, 0));
        
        var headingPara = new ParagraphLayout(_styles.Paragraph2) { Priority = 3 };
        textBox.AddParagraph(headingPara);
        
        headingPara.AddRun(new RunLayout(heading, _styles.RightSection));
        
        headingGrid.AddCell(new ElementCell(new BoxLayout
        {
            Height = 0.25f,
            FillColor = CvColors.Grey
        }, 2, 0));
        
        return headingGrid.Compile();
    }

    private TextBoxLayout BuildRightBlurb()
    {
        var textBox = new TextBoxLayout
        {
            InnerMargins = BlurbMargins,
            ParagraphSpacing = 0.5f
        };
        
        // We add a sneaky contextual que to LLMs to make it easier for them to find the blurb in the document.
        // This doesn't render in the visual document component. It's best to use this sparingly, as it can
        // look like keyword stuffing from the outside!
        var blurbTitlePara = new ParagraphLayout(_styles.Paragraph2) { Hidden = true, Priority = 1 };
        textBox.AddParagraph(blurbTitlePara);
        blurbTitlePara.AddRun(new RunLayout("Blurb", _styles.RightSection));

        var blurbPara = new ParagraphLayout(_styles.Paragraph2) { Priority = 1 };
        textBox.AddParagraph(blurbPara);
        
        blurbPara.AddRun(new RunLayout(_data.Profile.Blurb, _styles.RightParagraph));
        return textBox;
    }

    private CompiledGridLayout BuildLeftSkills(string heading, string[] skills)
    {
        var gridLayout = new TableBuilder(1);

        gridLayout.AddCell(new ElementCell(BuildLeftHeading(heading, 2), 0, 0));
        
        var textBox = new TextBoxLayout
        {
            InnerMargins = DefaultMargins,
            ParagraphSpacing = 0.5f
        };

        foreach (var skill in skills)
        {
            var skillPara = Para(2);
            textBox.AddParagraph(skillPara);
            skillPara.AddRun(new RunLayout(skill, _styles.LeftParagraph));
        }

        gridLayout.AddCell(new ElementCell(textBox, 1, 0));
        
        return gridLayout.Compile();
    }
    
    private CompiledGridLayout BuildLeftLanguages()
    {
        var gridLayout = new TableBuilder(1);

        gridLayout.AddCell(new ElementCell(BuildLeftHeading("Languages", 2), 0, 0));
        
        var textBox = new TextBoxLayout
        {
            InnerMargins = DefaultMargins,
            ParagraphSpacing = 0.5f
        };

        var sb = new StringBuilder();
        
        foreach (var language in _data.ProgrammingLanguages)
        {
            var skillPara = Para(2);
            textBox.AddParagraph(skillPara);

            sb.Append(language.Name);
            sb.Append("  ");
            for (var i = 0; i < (int)language.SkillLevel; i++)
            {
                sb.Append('◊');
            }
            
            skillPara.AddRun(new RunLayout(sb.ToString(), _styles.LeftParagraph));

            sb.Clear();
        }

        gridLayout.AddCell(new ElementCell(textBox, 1, 0));
        
        return gridLayout.Compile();
    }

    private TextBoxLayout BuildTitle()
    {
        var textBox = new TextBoxLayout
        {
            InnerMargins = NameMargins,
            ParagraphSpacing = -0.25f
        };
        
        var namePara = Para(0);
        textBox.AddParagraph(namePara);
        
        namePara.AddRun(new RunLayout(_data.Profile.Name, _styles.LeftName));

        var rolePara = Para(0);
        textBox.AddParagraph(rolePara);
        
        rolePara.AddRun(new RunLayout(_data.Profile.Role, _styles.LeftParagraph));
        
        return textBox;
    }

    private TextBoxLayout BuildLeftHeading(string heading, int priority = 0)
    {
        var textBox = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins,
            ParagraphSpacing = -0.25f,
            FillColor = _data.Style.SecondaryColor
        };
        
        var headingPara = Para(priority);
        textBox.AddParagraph(headingPara);
        
        headingPara.AddRun(new RunLayout(heading, _styles.LeftHeading));

        return textBox;
    }

    private TextBoxLayout BuildLeftContact()
    {
        var textBox = new TextBoxLayout
        {
            InnerMargins = DefaultMargins,
            ParagraphSpacing = 0.5f
        };
        
        AddDetail("Address", _data.Profile.Address);
        AddDetail("Phone", _data.Profile.Phone);
        AddDetail("Email", _data.Profile.Email);
        AddDetail("Linkedin", _data.Profile.LinkedIn);
        AddDetail("Github", _data.Profile.GitHub);
        
        return textBox;
        
        void AddDetail(string name, string value)
        {
            var detailPara = Para(0);
            textBox.AddParagraph(detailPara);
            detailPara.AddRun(new RunLayout(name, _styles.LeftParagraphBold));
            
            var responsePara = Para(0);
            textBox.AddParagraph(responsePara);
            responsePara.AddRun(new RunLayout(value, _styles.LeftParagraph));
        }
    }

    private PageLayout CreatePageTemplate(CompiledGridLayout gridLayout)
    {
        var pageLayout = new PageLayout(PageSizes.A4)
        {
            Grid = gridLayout,
            Background = new CanvasLayout(),
            OverflowBehaviour = PageOverflowBehaviour.PageFit,
            Margins = PageMargins
        };

        var backgroundGrid = new TableBuilder(1, 2);

        backgroundGrid.AddCell(new ElementCell(new BoxLayout
        {
            Height = PageSizes.A4.Height,
            FillColor = _data.Style.PrimaryColor
        }, 0, 0));
        
        pageLayout.Background.AddElement(backgroundGrid.Compile(), 0, 0, PageSizes.A4.Width, PageSizes.A4.Height);
        return pageLayout;
    }
    
    private ParagraphLayout Para(int priority = 0) => new(_styles.Paragraph) { Priority = priority };
}