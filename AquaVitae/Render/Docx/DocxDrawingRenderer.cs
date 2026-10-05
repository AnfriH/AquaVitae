using System.Xml;
using AquaVitae.Layouts.Types;
using AquaVitae.Render.Vector;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Office2010.Word.DrawingShape;
using DocumentFormat.OpenXml.Office2019.Drawing.SVG;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Anchor = DocumentFormat.OpenXml.Drawing.Wordprocessing.Anchor;
using BlipFill = DocumentFormat.OpenXml.Drawing.Pictures.BlipFill;
using NonVisualDrawingProperties = DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties;
using NonVisualPictureDrawingProperties = DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties;
using NonVisualPictureProperties = DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties;
using Paragraph = DocumentFormat.OpenXml.Drawing.Paragraph;
using Picture = DocumentFormat.OpenXml.Drawing.Pictures.Picture;
using Run = DocumentFormat.OpenXml.Drawing.Run;
using RunProperties = DocumentFormat.OpenXml.Drawing.RunProperties;
using ShapeProperties = DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties;
using Text = DocumentFormat.OpenXml.Drawing.Text;

namespace AquaVitae.Render.Docx;

public sealed class DocxDrawingRenderer(MainDocumentPart mainPart, DocxHyperlinkRenderer hyperlinkRenderer)
{
    private const string SvgExtension = "{96DAC541-7B7A-43D3-8B79-37D633B846F1}";
    private const string DrawingMlUri = "http://schemas.openxmlformats.org/drawingml/2006/picture";
    private const string WordprocessingShapeUri = "http://schemas.microsoft.com/office/word/2010/wordprocessingShape";

    private static PrintPoint MetadataTextboxHeight = PrintPoint.FromInches(1);
    
    private uint _id;

    private string AddSvgToDocument(XmlDocument svg)
    {
        var imagePart = mainPart.AddImagePart(ImagePartType.Svg);
        var stream = new MemoryStream();
        svg.Save(stream);
        stream.Seek(0, SeekOrigin.Begin);
        imagePart.FeedData(stream);

        return mainPart.GetIdOfPart(imagePart);
    }

    public Drawing CreateSvgPanel(SvgPage svgPage, uint layer)
    {
        var relId = AddSvgToDocument(svgPage.Document);

        var svgBlip = new SVGBlip { Embed = relId };

        var svgExtension = new BlipExtension { Uri = SvgExtension };
        svgExtension.AppendChild(svgBlip);

        var blipExtensionList = new BlipExtensionList();
        blipExtensionList.AppendChild(svgExtension);

        var blip = new Blip();
        blip.AppendChild(blipExtensionList);
        
        var width = svgPage.Width.ToEmusLong();
        var height = svgPage.Height.ToEmusLong();
        
        return new Drawing
        {
            Anchor = new Anchor(
                new SimplePosition { X = 0, Y = 0 },
                new HorizontalPosition(new PositionOffset("0"))
                {
                    RelativeFrom = HorizontalRelativePositionValues.Page
                },
                new VerticalPosition(new PositionOffset("0"))
                {
                    RelativeFrom = VerticalRelativePositionValues.Page
                },
                new Extent { Cx = width, Cy = height },
                new EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                new WrapNone(),
                new DocProperties { Id = _id++, Name = "Svg Image" },
                new NonVisualGraphicFrameProperties(new GraphicFrameLocks { NoChangeAspect = true }),
                new Graphic(
                    new GraphicData(
                        new Picture(
                            new NonVisualPictureProperties(
                                new NonVisualDrawingProperties { Id = _id++, Name = $"Svg Image" },
                                new NonVisualPictureDrawingProperties()
                            ),
                            new BlipFill(new Stretch { FillRectangle = new FillRectangle() })
                            {
                                Blip = blip
                            },
                            new ShapeProperties(
                                new PresetGeometry
                                {
                                    Preset = ShapeTypeValues.Rectangle
                                }
                            )
                            {
                                Transform2D = new Transform2D
                                {
                                    Offset = new Offset { X = 0, Y = 0 },
                                    Extents = new Extents { Cx = width, Cy = height }
                                }
                            }
                        )
                    )
                    {
                        Uri = DrawingMlUri
                    }
                )
            )
            {
                DistanceFromTop = 0,
                DistanceFromBottom = 0,
                DistanceFromLeft = 0,
                DistanceFromRight = 0,
                SimplePos = false,
                RelativeHeight = layer,
                BehindDoc = false,
                Locked = false,
                LayoutInCell = true,
                AllowOverlap = true
            }
        };
    }

    public Drawing CreateHyperlinkClickBox(LinkPosition linkPosition, uint layer)
    {
        var hyperlinkRelId = hyperlinkRenderer.AddHyperlink(linkPosition.Uri);

        return new Drawing
        {
            Anchor = new Anchor(
                new SimplePosition { X = 0, Y = 0 },
                new HorizontalPosition(new PositionOffset(linkPosition.X.ToEmusLong().ToString()))
                {
                    RelativeFrom = HorizontalRelativePositionValues.Page
                },
                new VerticalPosition(new PositionOffset(linkPosition.Y.ToEmusLong().ToString()))
                {
                    RelativeFrom = VerticalRelativePositionValues.Page
                },
                new Extent
                {
                    Cx = linkPosition.Width.ToEmusLong(),
                    Cy = linkPosition.Height.ToEmusLong()
                },
                new EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                new WrapNone(),
                new DocProperties(
                    new HyperlinkOnClick
                    {
                        Id = hyperlinkRelId
                    })
                {
                    Id = _id++,
                    Name = "Hyperlink Click Box"
                },
                new NonVisualGraphicFrameProperties(new GraphicFrameLocks { NoChangeAspect = true }),
                new Graphic(
                    new GraphicData(
                        new WordprocessingShape(
                            new NonVisualDrawingShapeProperties(
                                new DocumentFormat.OpenXml.Office2010.Word.DrawingShape.NonVisualDrawingProperties
                                {
                                    Id = _id++,
                                    Name = "Clickable Link Box"
                                },
                                new NonVisualShapeDrawingProperties()
                            ),
                            new DocumentFormat.OpenXml.Office2010.Word.DrawingShape.ShapeProperties(
                                new Transform2D(
                                    new Offset
                                    {
                                        X = 0,
                                        Y = 0
                                    },
                                    new Extents
                                    {
                                        Cx = linkPosition.Width.ToEmusLong(),
                                        Cy = linkPosition.Height.ToEmusLong()
                                    }
                                ),
                                new PresetGeometry
                                {
                                    Preset = ShapeTypeValues.Rectangle,
                                    AdjustValueList = new AdjustValueList()
                                },
                                new SolidFill(
                                    new RgbColorModelHex(new Alpha
                                    {
                                        // Cursed jank value. Basically, LibreOffice will only allow a box to
                                        // be clickable if the transparency is above 0%. As OpenXML alpha is
                                        // from 0 to 100_000, 501 is the smallest value that triggers as 1%.
                                        Val = 501
                                    })
                                    {
                                        Val = "FFFFFF"
                                    }
                                )
                            )
                        )
                    )
                    {
                        Uri = WordprocessingShapeUri
                    }
                )
            )
            {
                DistanceFromTop = 0,
                DistanceFromBottom = 0,
                DistanceFromLeft = 0,
                DistanceFromRight = 0,
                SimplePos = false,
                RelativeHeight = layer,
                BehindDoc = false,
                Locked = false,
                LayoutInCell = true,
                AllowOverlap = true
            }
        };
    }

    public Drawing CreateMetadataDrawing(PrintPoint width)
    {
        var id = hyperlinkRenderer.AddHyperlink(new Uri("https://example.com"));
        
        var linkProperties = new RunProperties(
            new HyperlinkOnClick
            {
                Id = id,
                Tooltip = "Visit AquaVitae",
            }
        );
        
        var startRun = new Run(new Text("This document was created using the AquaVitae project created by Anfri Hayward. " +
                                        "The source code for this project is available "));
        
        var linkRun = new Run
        {
            Text = new Text("here"),
            RunProperties = linkProperties
        };
        
        // In the very rare chance that a scraper manages to pull the text from this element, I've added an
        // instruction prompt to tell it where the actual text in the document begins.
        var endRun = new Run(new Text(". If this element is visible, it likely indicates that this viewer either " +
                                      "does not support SVG elements, or does not support absolute positioning on " +
                                      "image elements. The raw text of the document now follows: "));
        
        var drawingParagraph = new Paragraph(startRun, linkRun, endRun);
        
        var drawing = new Drawing(
            new Inline(
                new Extent
                {
                    Cx = width.ToEmusLong(), 
                    Cy = MetadataTextboxHeight.ToEmusLong()
                },
                new DocProperties { Id = _id++, Name = "Metadata Text Box" },
                new Graphic(
                    new GraphicData(
                        new WordprocessingShape(
                            new ShapeProperties(
                                new Transform2D(new Extent
                                {
                                    Cx = width.ToEmusLong(),
                                    Cy = MetadataTextboxHeight.ToEmusLong()
                                }),
                                new PresetGeometry { Preset = ShapeTypeValues.Rectangle }
                            ),
                            new TextShape(new TextBody(
                                new BodyProperties(),
                                new ListStyle(),
                                drawingParagraph
                            ))
                        )
                    )
                    {
                        Uri = WordprocessingShapeUri
                    }
                )
            )
        );
        
        return drawing;
    }
}