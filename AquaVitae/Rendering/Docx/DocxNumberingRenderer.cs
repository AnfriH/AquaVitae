using System.Diagnostics;
using AquaVitae.Common;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;
using AquaVitae.Layouts.Types;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Rendering.Docx;

public class DocxNumberingRenderer
{
    private readonly DocumentLayout _document;
    private readonly Numbering _numberingElement;
    private OptionalDictionary<string, AbstractNumEntry> _abstractEntries;
    
    private int _abstractId = 1;
    private int _numberId = 1;

    public DocxNumberingRenderer(DocumentLayout document, MainDocumentPart documentPart)
    {
        _document = document;
        var numberingPart = documentPart.AddNewPart<NumberingDefinitionsPart>();
        numberingPart.Numbering = new Numbering();
        _numberingElement = numberingPart.Numbering;
    }
    
    public int GetNumberingId(string? styleId, int level)
    {
        if (styleId == null) return 0;
        
        var style = _document.VerticalListStyles[styleId];
        var entry = GetAbstractEntry(style);

        if (!entry.Levels.Add(level)) return entry.NumberId;

        // TODO: add more styling configuration
        var layer = level + 1;
        var hanging = style.FirstIndentation / 2;
        var left = hanging + style.FollowingIndentation * layer;
        
        
        var levelElement = new Level
        {
            LevelIndex = level,
            PreviousParagraphProperties = new PreviousParagraphProperties
            {
                Indentation = new Indentation
                {
                    Left = left.ToTwipsString(),
                    Hanging = hanging.ToTwipsString()
                },
                Tabs = new Tabs(new TabStop
                    {
                        Val = TabStopValues.Center,
                        Position = left.ToTwipsInt()
                    }
                )
            }
        };

        switch (style.Type)
        {
            case VerticalListType.Unordered:
                levelElement.NumberingFormat = new NumberingFormat { Val = NumberFormatValues.Bullet };
                levelElement.LevelText = new LevelText { Val = "-" };
                levelElement.LevelJustification = new LevelJustification { Val = LevelJustificationValues.Center };
                break;
            case VerticalListType.Ordered:
                levelElement.NumberingFormat = new NumberingFormat { Val = NumberFormatValues.Decimal };
                levelElement.StartNumberingValue = new StartNumberingValue { Val = 1 };
                levelElement.LevelText = new LevelText { Val = $"%{layer}." };
                levelElement.LevelJustification = new LevelJustification { Val = LevelJustificationValues.Center };
                break;
            default:
                throw new UnreachableException();
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
        if (style.Type == VerticalListType.Unordered) return entry;
        
        // Otherwise, we're ordered, therefore we need to create a new instance
        AppendNumberingInstance(_numberId++, entry.AbstractId);
        return entry;
    }

    private AbstractNumEntry AppendAbstractNum(int numberId, int abstractId)
    {
        var element = new AbstractNum
        {
            AbstractNumberId = abstractId,
            MultiLevelType = new MultiLevelType { Val = MultiLevelValues.HybridMultilevel }
        };
        _numberingElement.AppendChild(element);
        
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
        _numberingElement.AppendChild(element);
    }

    private class AbstractNumEntry(AbstractNum element, int abstractId, int numberId)
    {
        public AbstractNum Element => element;
        public HashSet<int> Levels { get; } = [];
        public int AbstractId { get; set; } = abstractId;
        public int NumberId { get; set; } = numberId;
    }
}