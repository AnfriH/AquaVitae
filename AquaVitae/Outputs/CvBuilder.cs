using AquaVitae.Data;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Abstractions;
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

    private static readonly Margins DefaultMargins = new(18, 18, 18, 18);
    private static readonly Margins LeftHeadingMargins = new(3, -6, 18, 18);
    
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
        _documentLayout.AddPage(BuildSecondPage());

        return _documentLayout;
    }

    private PageLayout BuildFirstPage()
    {
        var pageLayout = CreatePageTemplate();
        var grid = pageLayout.Grid!;
        
        grid.AddCell(new GridCell(BuildTitle(), 0, 0));
        grid.AddCell(new GridCell(BuildLeftHeading("Contact"), 1, 0));
        grid.AddCell(new GridCell(BuildLeftContact(), 2, 0));
        grid.AddCell(new GridCell(BuildLeftSkills("Technical Skills", _data.Skills.Technical), 3, 0));
        grid.AddCell(new GridCell(new BoxLayout { Height = 72}, 4, 0));
        grid.AddCell(new GridCell(new BoxLayout { Height = 36}, 5, 0));
        grid.AddCell(new GridCell(BuildLeftSkills("Soft Skills", _data.Skills.Soft), 6, 0));
        
        // Right side
        var rightGrid = new GridLayout(1);
        grid.AddCell(new GridCell(rightGrid, 0, 1) { RowSpan = 5 });
        
        rightGrid.AddCell(new GridCell(BuildRightBlurb(), 0, 0));
        rightGrid.AddCell(new GridCell(BuildRightHeading("Work History"), 1, 0));
        
        rightGrid.AddCell(new GridCell(BuildRightExperience(_data.Experience[0]), 2, 0));
        rightGrid.AddCell(new GridCell(BuildRightExperience(_data.Experience[1]), 3, 0));
        
        var rightGrid2 = new GridLayout(1);
        
        grid.AddCell(new GridCell(new BoxLayout { Height = 36}, 5, 1));
        
        grid.AddCell(new GridCell(rightGrid2, 6, 1) { RowSpan = 1 });
        
        // FIXME: Currently, these are hardcoded. We should instead take advantage of the layout system to automatically
        // split this list when it gets too long for a single page.
        rightGrid2.AddCell(new GridCell(BuildRightExperience(_data.Experience[2]), 0, 0));
        rightGrid2.AddCell(new GridCell(BuildRightExperience(_data.Experience[3]), 1, 0));
        rightGrid2.AddCell(new GridCell(BuildRightExperience(_data.Experience[4]), 2, 0));
        rightGrid2.AddCell(new GridCell(new BoxLayout { Height = 18}, 3, 0));
        
        rightGrid2.AddCell(new GridCell(BuildRightHeading("Education"), 4, 0));
        rightGrid2.AddCell(new GridCell(new BoxLayout { Height = 18}, 5, 0));
        
        rightGrid2.AddCell(new GridCell(BuildRightEducation(_data.Education[0]), 6, 0));
        rightGrid2.AddCell(new GridCell(BuildRightEducation(_data.Education[1]), 7, 0));
        
        return pageLayout;
    }

    private ElementLayoutBase BuildRightExperience(ExperienceData experience)
    {
        var experienceGrid = new GridLayout(2, 6);

        var titleTextbox = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        var titlePara = new ParagraphLayout(_styles.Paragraph2) { Priority = 1 };
        titlePara.AddRun(new RunLayout(experience.Role, _styles.RightHeading));
        titleTextbox.AddParagraph(titlePara);

        experienceGrid.AddCell(new GridCell(titleTextbox, 0, 1));
        
        var datesLayout = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        
        var datesPara = new ParagraphLayout(_styles.Paragraph2) { Priority = 1 };
        datesPara.AddRun(new RunLayout($"{experience.GetStartDate():yyyy-MM} - {experience.GetEndDate()?.ToString("yyyy-MM") ?? "current"}", _styles.RightParagraph));
        
        datesLayout.AddParagraph(datesPara);
        
        experienceGrid.AddCell(new GridCell(datesLayout, 0, 0) { RowSpan = 2 });

        var roleLayout = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        var rolePara = new ParagraphLayout(_styles.Paragraph2) { Priority = 1 };
        roleLayout.AddParagraph(rolePara);
        
        rolePara.AddRun(new RunLayout(experience.Company, _styles.RightParagraph));
        experienceGrid.AddCell(new GridCell(roleLayout, 1, 1));
        
        var respLayout = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        
        
        var respPara = new VerticalListLayout(_styles.Paragraph2, _styles.RightList) { Priority = 1 };
        respLayout.AddParagraph(respPara);

        foreach (var resp in experience.Responsibilities)
        {
            var itemPara = new ParagraphLayout(_styles.Paragraph2);
            itemPara.AddRun(new RunLayout(resp, _styles.RightParagraph));
            respPara.AddParagraph(itemPara);
        }
        experienceGrid.AddCell(new GridCell(respLayout, 2, 1));

        return experienceGrid;
    }
    
    private ElementLayoutBase BuildRightEducation(EducationData educationData)
    {
        var experienceGrid = new GridLayout(2, 6);

        var titleTextbox = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        var titlePara = new ParagraphLayout(_styles.Paragraph2) { Priority = 1 };
        titlePara.AddRun(new RunLayout(educationData.Qualification, _styles.RightHeading));
        titleTextbox.AddParagraph(titlePara);

        experienceGrid.AddCell(new GridCell(titleTextbox, 0, 1));
        
        var datesLayout = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        
        var datesPara = new ParagraphLayout(_styles.Paragraph2) { Priority = 1 };
        datesPara.AddRun(new RunLayout($"{educationData.GetCompletedDate():yyyy-MM}", _styles.RightParagraph));
        
        datesLayout.AddParagraph(datesPara);
        
        experienceGrid.AddCell(new GridCell(datesLayout, 0, 0) { RowSpan = 2 });

        var roleLayout = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        var rolePara = new ParagraphLayout(_styles.Paragraph2) { Priority = 1 };
        roleLayout.AddParagraph(rolePara);
        
        rolePara.AddRun(new RunLayout(educationData.Institution, _styles.RightParagraph));
        experienceGrid.AddCell(new GridCell(roleLayout, 1, 1));

        return experienceGrid;
    }

    private GridLayout BuildRightHeading(string heading)
    {
        var headingGrid = new GridLayout(1);

        headingGrid.AddCell(new GridCell(new BoxLayout
        {
            Height = 0.25f,
            FillColor = CvColors.Grey
        }, 0, 0));

        var textBox = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins
        };
        headingGrid.AddCell(new GridCell(textBox, 1, 0));
        
        var headingPara = Para();
        textBox.AddParagraph(headingPara);
        
        headingPara.AddRun(new RunLayout(heading, _styles.RightSection));
        
        headingGrid.AddCell(new GridCell(new BoxLayout
        {
            Height = 0.25f,
            FillColor = CvColors.Grey
        }, 2, 0));
        
        return headingGrid;
    }

    private TextBoxLayout BuildRightBlurb()
    {
        var textBox = new TextBoxLayout
        {
            InnerMargins = DefaultMargins,
            ParagraphSpacing = 0.5f
        };

        var blurbPara = new ParagraphLayout(_styles.Paragraph2);
        textBox.AddParagraph(blurbPara);
        
        blurbPara.AddRun(new RunLayout(_data.Profile.Blurb, _styles.RightParagraph));
        return textBox;
    }

    private GridLayout BuildLeftSkills(string heading, string[] skills)
    {
        var gridLayout = new GridLayout(1);

        gridLayout.AddCell(new GridCell(BuildLeftHeading(heading), 0, 0));
        
        var textBox = new TextBoxLayout
        {
            InnerMargins = DefaultMargins,
            ParagraphSpacing = 0.5f
        };

        foreach (var skill in skills)
        {
            var skillPara = Para();
            textBox.AddParagraph(skillPara);
            skillPara.AddRun(new RunLayout(skill, _styles.LeftParagraph));
        }

        gridLayout.AddCell(new GridCell(textBox, 1, 0));
        
        return gridLayout;
    }

    private TextBoxLayout BuildTitle()
    {
        var textBox = new TextBoxLayout
        {
            InnerMargins = DefaultMargins,
            ParagraphSpacing = -0.25f
        };
        
        var namePara = Para();
        textBox.AddParagraph(namePara);
        
        namePara.AddRun(new RunLayout(_data.Profile.Name, _styles.LeftName));

        var rolePara = Para();
        textBox.AddParagraph(rolePara);
        
        rolePara.AddRun(new RunLayout(_data.Profile.Role, _styles.LeftParagraph));
        
        return textBox;
    }

    private TextBoxLayout BuildLeftHeading(string heading)
    {
        var textBox = new TextBoxLayout
        {
            InnerMargins = LeftHeadingMargins,
            ParagraphSpacing = -0.25f,
            FillColor = _data.Style.SecondaryColor
        };
        
        var headingPara = Para();
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
            var detailPara = Para();
            textBox.AddParagraph(detailPara);
            detailPara.AddRun(new RunLayout(name, _styles.LeftParagraphBold));
            
            var responsePara = Para();
            textBox.AddParagraph(responsePara);
            responsePara.AddRun(new RunLayout(value, _styles.LeftParagraph));
        }
    }
    
    private PageLayout BuildSecondPage()
    {
        var pageLayout = CreatePageTemplate();
        return pageLayout;
    }

    private PageLayout CreatePageTemplate()
    {
        var pageLayout = new PageLayout(PageSizes.A4)
        {
            Grid = new GridLayout(1, 2), // divided into 1/3 and 2/3 sections,
            Background = new CanvasLayout(),
            OverflowBehaviour = PageOverflowBehaviour.PageFit
        };

        var backgroundGrid = new GridLayout(1, 2);

        backgroundGrid.AddCell(new GridCell(new BoxLayout
        {
            Width = PageSizes.A4.Width / 3,
            Height = PageSizes.A4.Height,
            FillColor = _data.Style.PrimaryColor
        }, 0, 0));
        
        pageLayout.Background.AddElement(backgroundGrid, 0, 0, PageSizes.A4.Width, PageSizes.A4.Height);
        return pageLayout;
    }
    
    private ParagraphLayout Para() => new(_styles.Paragraph);
}