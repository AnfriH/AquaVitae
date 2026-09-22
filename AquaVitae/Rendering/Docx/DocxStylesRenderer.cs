using System.Diagnostics;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;
using AquaVitae.Layouts.Types;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;
using RunStyle = AquaVitae.Layouts.Styles.RunStyle;

namespace AquaVitae.Rendering.Docx;

public sealed class DocxStylesRenderer
{
    private readonly Dictionary<string, StyleRunProperties> _runStyles = new();
    private readonly HashSet<string> _runStyleParagraphBases = [];
    
    public Styles RenderStyles(DocumentLayout document)
    {
        var stylesElement = new Styles();
        
        foreach (var style in document.RunStyles.Values)
        {
            var styleElement = stylesElement.AppendChild(CreateBaseStyle(style.Name, style.Id, StyleValues.Character));
            styleElement.StyleRunProperties = CreateRunStyleLayout(style);
            _runStyles[style.Id] = styleElement.StyleRunProperties;
        }

        foreach (var style in document.ParagraphStyles.Values)
        {
            var styleElement = stylesElement.AppendChild(
                CreateBaseStyle(style.Name, style.Id!, StyleValues.Paragraph)
            );
            styleElement.StyleParagraphProperties = CreateParagraphStyleLayout(style);

            if (style.RunId.Value == null) continue;
            
            // If the paragraph style has a run style, we create a base paragraph style
            // for it to inherit the run style from. This reduces the XML size a fair bit.
            // TODO: Consider allowing styles to inherit from other styles explicitly
            var runStyle = _runStyles[style.RunId.Value!];
            
            var baseId = style.RunId + "_BASE";
            if (_runStyleParagraphBases.Add(baseId))
            {
                var runBaseElement = stylesElement.AppendChild(
                    CreateBaseStyle(baseId + " Base", baseId, StyleValues.Paragraph)
                );
                runBaseElement.StyleRunProperties = (StyleRunProperties)runStyle.CloneNode(true);
            }
            
            styleElement.BasedOn = new BasedOn { Val = baseId };
        }

        return stylesElement;
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

    private static StyleParagraphProperties CreateParagraphStyleLayout(ParagraphStyle style)
    {
        var justification = style.Alignment switch
        {
            ParagraphAlignment.Left => JustificationValues.Left,
            ParagraphAlignment.Right => JustificationValues.Right,
            ParagraphAlignment.Center => JustificationValues.Center,
            ParagraphAlignment.Justify => JustificationValues.Both,
            _ => throw new UnreachableException()
        };

        return new StyleParagraphProperties
        {
            Justification = new Justification
            {
                Val = justification
            }
        };
    }

    private static StyleRunProperties CreateRunStyleLayout(RunStyle style)
    {
        var properties = new StyleRunProperties();

        var font = style.FontName;
        if (font != null)
        {
            properties.RunFonts = new RunFonts
            {
                Ascii = font,
                HighAnsi = font
            };
        }
        
        var fontsize = style.FontSize;
        if (fontsize != null) properties.FontSize = new FontSize { Val = fontsize.Value.ToHalfPointsString() };
        
        var color = style.Color;
        if (color != null) properties.Color = new Color { Val = color.Value.ToString(ColorFormats.Rgb) };
        
        if (style.Bold) properties.Bold = new Bold { Val = true };
        if (style.Italic) properties.Italic = new Italic { Val = true };
        if (style.Underline) properties.Underline = new Underline { Val = UnderlineValues.Single };
        if (style.Strikethrough) properties.Strike = new Strike { Val = true };
        
        return properties;
    }
}