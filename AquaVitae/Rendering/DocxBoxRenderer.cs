using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Rendering;

public sealed class DocxBoxRenderer(DocxParagraphRenderer paragraphRenderer)
{
    public async Task RenderAsync(BoxLayout box, Body body)
    {
        var tableCell = RenderTableCell(box);

        // TODO: allow other elements inside cells
        foreach (var paragraph in box.Paragraphs)
        {
            await paragraphRenderer.RenderAsync(paragraph, tableCell);
        }

        // If the box has no contents, we must add an empty paragraph
        if (box.Paragraphs.Count == 0)
        {
            tableCell.AppendChild(new Paragraph());
        }

        var table = RenderTable(box, tableCell);
        body.AppendChild(table);
    }
    
    private TableCell RenderTableCell(BoxLayout box)
    {
        var innerMargins = box.InnerMargins;

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

        var fillColor = box.FillColor;
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

    private Table RenderTable(BoxLayout box, TableCell cell)
    {
        var outerMargins = box.OuterMargins;
        var tableProps = new TableProperties
        {
            TableWidth = new TableWidth
            {
                Width = box.Width.ToTwipsString(),
                Type = TableWidthUnitValues.Dxa
            },
            TablePositionProperties = new TablePositionProperties
            {
                TopFromText = outerMargins.Top.ToTwipShort(),
                BottomFromText = outerMargins.Bottom.ToTwipShort(),
                LeftFromText = outerMargins.Left.ToTwipShort(),
                RightFromText = outerMargins.Right.ToTwipShort(),
            
                VerticalAnchor = VerticalAnchorValues.Text,
                HorizontalAnchor = HorizontalAnchorValues.Text,
            
                // TODO: Work out what is actually padding this by 108 Dxa
                TablePositionX = box.X.ToTwipsInt() + 108,
                TablePositionY = box.Y.ToTwipsInt()
            },
            TableOverlap = new TableOverlap { Val = TableOverlapValues.Overlap }
        };

        var table = new Table
        {
            TableProperties = tableProps,
        };
        
        var gridColumn = new GridColumn
        {
            Width = box.Width.ToTwipsString(),
        };
        var tableGrid = new TableGrid();
        tableGrid.AppendChild(gridColumn);
        
        table.TableGrid = tableGrid;

        var row = new TableRow
        {
            TableRowProperties = [
                with(new TableRowHeight
                {
                    Val = box.Height.ToTwipsUInt(),
                    HeightType = HeightRuleValues.Exact
                })
            ]
        };
        
        row.AppendChild(cell);
        table.AppendChild(row);
        
        return table;
    }
}