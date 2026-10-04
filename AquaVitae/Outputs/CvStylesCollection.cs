using AquaVitae.Data;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Outputs;

public sealed record CvStylesCollection(
    StyleId<RunStyle> LeftParagraph,
    StyleId<RunStyle> LeftParagraphBold,
    StyleId<RunStyle> LeftHeading,
    StyleId<RunStyle> LeftName,
    StyleId<RunStyle> RightParagraph,
    StyleId<RunStyle> RightHeading,
    StyleId<RunStyle> RightSection,
    StyleId<ParagraphStyle> Paragraph,
    StyleId<ParagraphStyle> Paragraph2,
    StyleId<VerticalListStyle> LeftList,
    StyleId<VerticalListStyle> RightList
)
{
    public static CvStylesCollection Build(
        StyleLayout styleLayout,
        StyleData styleData
    )
    {
        var font = new FontStyle(styleData.FontName, $"font_{styleData.FontName}")
        {
            RegularFile = styleData.FontPathRegular,
            BoldFile = styleData.FontPathBold
        };
        
        var fontId = styleLayout.RegisterStyle(font);
        
        var leftParagraphStyle = new RunStyle("Left Paragraph", "left_paragraph", fontId)
        {
            FontSize = 12,
            Color = CvColors.White
        };
        
        var leftParagraphBoldStyle = new RunStyle("Left Paragraph Bold", "left_paragraph_bold", fontId)
        {
            FontSize = 12,
            Color = CvColors.White,
            Bold = true
        };

        var leftHeadingStyle = leftParagraphStyle with
        {
            Name = "Left Heading",
            Id = "left_heading",
            FontSize = 18,
            Bold = true
        };
        
        var leftNameStyle = leftHeadingStyle with
        {
            Name = "Left Name",
            Id = "left_name",
            FontSize = 28
        };

        var rightParagraphStyle = new RunStyle("Right Paragraph", "right_paragraph", fontId)
        {
            FontSize = 10,
            Color = CvColors.Grey
        };

        var rightHeadingStyle = rightParagraphStyle with
        {
            Name = "Right Heading",
            Id = "right_heading",
            FontSize = 13,
            Bold = true
        };

        var rightSectionStyle = new RunStyle("Right Section", "right_section", fontId)
        {
            FontSize = 16,
            Color = styleData.SecondaryColor,
            Bold = true
        };
        
        var paragraphStyle = new ParagraphStyle("Standard Paragraph", "standard_paragraph")
        {
            LineSpacing = -0.25f
        };
        
        var paragraphStyle2 = new ParagraphStyle("Standard Paragraph 2", "standard_paragraph_2")
        {
            LineSpacing = 0f
        };

        var leftListStyle = new VerticalListStyle("Left List", "left_list")
        {
            Ordered = false,
            ElementColor = styleData.PrimaryColor,
            Indent = 0 // This makes the bullet points invisible
        };
        
        var rightListStyle = new VerticalListStyle("Right List", "right_list")
        {
            Ordered = false,
            ElementColor = CvColors.Grey,
            Indent = 2.5f
        };

        return new CvStylesCollection(
            styleLayout.RegisterStyle(leftParagraphStyle),
            styleLayout.RegisterStyle(leftParagraphBoldStyle),
            styleLayout.RegisterStyle(leftHeadingStyle),
            styleLayout.RegisterStyle(leftNameStyle),
            styleLayout.RegisterStyle(rightParagraphStyle),
            styleLayout.RegisterStyle(rightHeadingStyle),
            styleLayout.RegisterStyle(rightSectionStyle),
            styleLayout.RegisterStyle(paragraphStyle),
            styleLayout.RegisterStyle(paragraphStyle2),
            styleLayout.RegisterStyle(leftListStyle),
            styleLayout.RegisterStyle(rightListStyle)
        );
    }
}