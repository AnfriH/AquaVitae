using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;
using AquaVitae.Layouts.Types;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;
using RunStyle = AquaVitae.Layouts.Styles.RunStyle;

namespace AquaVitae.Render.Docx;

public sealed class DocxStylesRenderer(
    DocumentLayout documentLayout,
    MainDocumentPart mainPart,
    DocxRendererSettings settings
)
{
    private readonly Dictionary<string, Style> _runStyles = new();
    private readonly Dictionary<string, Style> _paragraphStyles = new();
    
    private Styles StylesElement {
        get
        {
            if (field != null) return field;
            field = new Styles();
            
            var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            stylesPart.Styles = field;
            return field;
        }
    }

    public void AddRunStyle(StyleId<RunStyle> style)
    {
        RenderRunStyle(documentLayout.Styles.GetStyle(style));
    }
    
    private Style RenderRunStyle(RunStyle style)
    {
        if (_runStyles.TryGetValue(style.Id, out var styleElement)) return styleElement;
        styleElement = CreateBaseStyle(style.Name, style.Id, StyleValues.Character);
        styleElement.StyleRunProperties = CreateRunStyleLayout(style);
        
        _runStyles[style.Id] = styleElement;
        StylesElement.AppendChild(styleElement);
        
        return styleElement;
    }
    
    public void AddParagraphStyle(StyleId<ParagraphStyle> styleId)
    {
        RenderParagraphStyle(documentLayout.Styles.GetStyle(styleId));
    }

    private Style RenderParagraphStyle(ParagraphStyle style)
    {
        if (_paragraphStyles.TryGetValue(style.Id, out var styleElement)) return styleElement;
        
        styleElement = CreateBaseStyle(style.Name, style.Id, StyleValues.Paragraph);
        styleElement.StyleParagraphProperties = CreateParagraphStyleLayout(style);
        StylesElement.AppendChild(styleElement);
        
        if (style.RunStyle.IsNone) return styleElement;
            
        // If the paragraph style has a run style, we create a base paragraph style
        // for it to inherit the run style from. This reduces the XML size a fair bit.
        var runStyle = documentLayout.Styles.GetStyle(style.RunStyle);
        
        var baseId = runStyle.Id + "_BASE";
        if (!_paragraphStyles.ContainsKey(baseId))
        {
            var runBaseElement = StylesElement.AppendChild(
                CreateBaseStyle(runStyle.Name + " Base", baseId, StyleValues.Paragraph)
            );
            
            var runStyleElement = RenderRunStyle(runStyle);
            
            runBaseElement.StyleRunProperties = (StyleRunProperties)runStyleElement.StyleRunProperties!.CloneNode(true);
            
            _paragraphStyles[baseId] = runBaseElement;
        }
            
        styleElement.BasedOn = new BasedOn { Val = baseId };
        return styleElement;
    }

    private static Style CreateBaseStyle(string name, string id, StyleValues type)
    {
        return new Style
        {
            StyleName = new StyleName { Val = name },
            StyleId = id,
            CustomStyle = true,
            Type = type
        };
    }

    private static StyleParagraphProperties CreateParagraphStyleLayout(ParagraphStyle _)
    {
        var properties = new StyleParagraphProperties
        {
            // We set the line spacing to the smallest possible non-zero value.
            // The resulting effect is that all the text is smushed together
            // onto more or less a single line. This helps to ensure that the
            // text does not extend beyond the final page.
            SpacingBetweenLines = new SpacingBetweenLines
            {
                Line = "1",
                LineRule = LineSpacingRuleValues.Exact,
                Before = "0",
                After = "0",
                AfterAutoSpacing = false,
                BeforeAutoSpacing = false
            }
        };

        return properties;
    }

    private StyleRunProperties CreateRunStyleLayout(RunStyle style)
    {
        var properties = new StyleRunProperties();

        var fontId = style.Font;
        var font = documentLayout.Styles.GetStyle(fontId);
        properties.RunFonts = new RunFonts
        {
            Ascii = font.Name,
            HighAnsi = font.Name
        };
        
        properties.FontSize = new FontSize { Val = (style.FontSize / 4).ToHalfPointsString() };

        if (settings.IncludeTextColor)
        {
            properties.Color = new Color { Val = style.Color.ToString(ColorFormats.Rgb) };
        }
        
        if (style.Bold) properties.Bold = new Bold { Val = true };
        if (style.Italic) properties.Italic = new Italic { Val = true };
        if (style.Underline) properties.Underline = new Underline { Val = UnderlineValues.Single };
        if (style.Strikethrough) properties.Strike = new Strike { Val = true };
        
        return properties;
    }
}