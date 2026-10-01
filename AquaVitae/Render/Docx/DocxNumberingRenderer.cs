using AquaVitae.Common;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public class DocxNumberingRenderer(DocumentLayout document, MainDocumentPart documentPart)
{
    private Numbering NumberingElement
    {
        get {
            if (field != null) return field;
            field = new Numbering();
            
            var numberingPart = documentPart.AddNewPart<NumberingDefinitionsPart>();
            numberingPart.Numbering = field;
            return field;
        }
    }
    private OptionalDictionary<string, AbstractNumEntry> _abstractEntries;
    
    private int _abstractId = 1;
    private int _numberId = 1;
    
    public int GetNumberingId(StyleId<VerticalListStyle> styleId, int level)
    {
        var style = document.Styles.GetStyle(styleId);
        var entry = GetAbstractEntry(style);

        if (!entry.Levels.Add(level)) return entry.NumberId;
        
        var left = style.Indent * level;
        
        var ordered = style.Ordered;
        
        var levelElement = new Level
        {
            LevelText = new LevelText { Val = ordered ? $"%{level}." : "•" },
            LevelIndex = level,
            LevelSuffix = new LevelSuffix { Val = LevelSuffixValues.Space },
            LevelJustification = new LevelJustification { Val = LevelJustificationValues.Left },
            PreviousParagraphProperties = new PreviousParagraphProperties
            {
                Indentation = new Indentation
                {
                    Left = left.ToTwipsString()
                }
            }
        };

        if (ordered)
        {
            levelElement.NumberingFormat = new NumberingFormat { Val = NumberFormatValues.Decimal };
            levelElement.StartNumberingValue = new StartNumberingValue { Val = 1 };
        }
        else
        {
            levelElement.NumberingFormat = new NumberingFormat { Val = NumberFormatValues.Bullet };
        }
        
        entry.Element.AppendChild(levelElement);

        return entry.NumberId;
    }

    private AbstractNumEntry GetAbstractEntry(VerticalListStyle style)
    {
        if (!_abstractEntries.TryGetValue(style.Id, out var entry))
        {
            entry = AppendAbstractNum(_numberId++, _abstractId++);
            _abstractEntries[style.Id] = entry;
            return entry;
        }

        // If we're unordered, we can reuse the existing numbering instance
        // if (!style.MarkerStyle.Ordered) return entry;
        
        // Otherwise, we're ordered, therefore we need to create a new instance
        AppendNumberingInstance(_numberId++, entry.AbstractId);
        return entry;
    }

    private AbstractNumEntry AppendAbstractNum(int numberId, int abstractId)
    {
        var element = new AbstractNum
        {
            AbstractNumberId = abstractId
        };
        NumberingElement.AppendChild(element);
        
        AppendNumberingInstance(numberId, abstractId);

        return new AbstractNumEntry(
            element,
            numberId,
            abstractId
        );
    }

    private void AppendNumberingInstance(int numberId, int abstractId)
    {
        var element = new NumberingInstance
        {
            AbstractNumId = new AbstractNumId { Val = abstractId },
            NumberID = numberId
        };
        NumberingElement.AppendChild(element);
    }

    private class AbstractNumEntry(AbstractNum element, int abstractId, int numberId)
    {
        public AbstractNum Element => element;
        public HashSet<int> Levels { get; } = [];
        public int AbstractId { get; set; } = abstractId;
        public int NumberId { get; set; } = numberId;
    }
}