using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public sealed class DocxBoxRenderer(
    Lazy<DocxNumberingRenderer> numberingRenderer,
    Lazy<DocxHyperlinkRenderer> hyperlinkRenderer
)
{
    private DocxParagraphRenderer ParagraphRenderer => field ??= new DocxParagraphRenderer(hyperlinkRenderer);
    private DocxVerticalListRenderer VerticalListRenderer => field ??= new DocxVerticalListRenderer(
        numberingRenderer.Value,
        ParagraphRenderer
    );
    
    public Table RenderBox(TextBoxLayout textBox)
    {
        var tableCell = RenderTableCell(textBox);

        // TODO: allow other elements inside cells
        foreach (var child in textBox.Paragraphs)
        {
            switch (child)
            {
                case ParagraphLayout paragraph:
                    tableCell.AppendChild(ParagraphRenderer.RenderParagraph(paragraph));
                    break;
                case VerticalListLayout vertList:
                    VerticalListRenderer.RenderVerticalList(vertList, tableCell);
                    break;
            }
        }

        // If the textbox has no contents, we must add an empty paragraph
        if (textBox.Paragraphs.Count == 0)
        {
            tableCell.AppendChild(new Paragraph());
        }
        
        return RenderTable(textBox, tableCell);
    }
    
    private static TableCell RenderTableCell(TextBoxLayout textBox)
    {
        var innerMargins = textBox.InnerMargins;

        var tableCellProperties = new TableCellProperties
        {
            TableCellMargin = new TableCellMargin
            {
                TopMargin = new TopMargin
                {
                    Width = innerMargins.Top.ToTwipsString(),
                    Type = TableWidthUnitValues.Dxa
                },
                BottomMargin = new BottomMargin
                {
                    Width = innerMargins.Bottom.ToTwipsString(),
                    Type = TableWidthUnitValues.Dxa
                },
                LeftMargin = new LeftMargin
                {
                    Width = innerMargins.Left.ToTwipsString(),
                    Type = TableWidthUnitValues.Dxa
                },
                RightMargin = new RightMargin
                {
                    Width = innerMargins.Right.ToTwipsString(),
                    Type = TableWidthUnitValues.Dxa
                }
            }
        };

        var fillColor = textBox.FillColor;
        if (fillColor.HasValue)
        {
            tableCellProperties.Shading = new Shading
            {
                Val = ShadingPatternValues.Clear,
                Fill = fillColor.Value.ToString(ColorFormats.Rgb)
            };
        }
        
        return new TableCell
        {
            TableCellProperties = tableCellProperties
        };
    }

    private static Table RenderTable(TextBoxLayout textBox, TableCell cell)
    {
        // For a reason I've still not been able to work out,
        // floating tables are consistently offset by -108 twips.
        var x = textBox.X + PrintPoint.FromTwips(108);
        
        // Due to a table layering bug in OpenXML, we cannot have tables whose
        // y position exactly aligns with the top of the page.
        var y = textBox.Y != 0 ? textBox.Y : PrintPoint.FromTwips(1);
        
        var tableProps = new TableProperties
        {
            TableWidth = new TableWidth
            {
                Width = textBox.Width.ToTwipsString(),
                Type = TableWidthUnitValues.Dxa
            },
            TablePositionProperties = new TablePositionProperties
            {
                TopFromText = 0,
                BottomFromText = 0,
                LeftFromText = 0,
                RightFromText = 0,
            
                VerticalAnchor = VerticalAnchorValues.Page,
                HorizontalAnchor = HorizontalAnchorValues.Page,
                
                TablePositionX = x.ToTwipsInt(),
                TablePositionY = y.ToTwipsInt()
            },
            TableOverlap = new TableOverlap { Val = TableOverlapValues.Overlap }
        };

        var table = new Table
        {
            TableProperties = tableProps,
        };
        
        var gridColumn = new GridColumn
        {
            Width = textBox.Width.ToTwipsString(),
        };
        var tableGrid = new TableGrid();
        tableGrid.AppendChild(gridColumn);
        
        table.TableGrid = tableGrid;

        var rowProperties = new TableRowProperties();
        
        var boxHeight = textBox.Height;
        if (boxHeight.Points <= 0)
        {
            rowProperties.AppendChild(new TableRowHeight
            {
                Val = 0,
                HeightType = HeightRuleValues.Auto
            });
        }
        else
        {
            rowProperties.AppendChild(new TableRowHeight
            {
                Val = boxHeight.ToTwipsUInt(),
                HeightType = HeightRuleValues.Exact
            });
        }
        rowProperties.AppendChild(new CantSplit { Val = OnOffOnlyValues.On });
        
        var row = new TableRow { TableRowProperties = rowProperties };
        
        row.AppendChild(cell);
        table.AppendChild(row);
        
        return table;
    }
}