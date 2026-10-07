using System.Xml;
using AquaVitae.Layouts.Types;
using AquaVitae.Render.Vector;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

using Drawing = DocumentFormat.OpenXml.Drawing;
using DrawingPictures = DocumentFormat.OpenXml.Drawing.Pictures;
using WordProcessingDrawing = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DrawingShape = DocumentFormat.OpenXml.Office2010.Word.DrawingShape;
using SVG = DocumentFormat.OpenXml.Office2019.Drawing.SVG;
using WordProcessing = DocumentFormat.OpenXml.Wordprocessing;

namespace AquaVitae.Render.Docx;

public sealed class DocxDrawingRenderer(MainDocumentPart mainPart, DocxHyperlinkRenderer hyperlinkRenderer)
{
    private const string SvgExtension = "{96DAC541-7B7A-43D3-8B79-37D633B846F1}";
    private const string DrawingMlPictureUri = "http://schemas.openxmlformats.org/drawingml/2006/picture";
    private const string DrawingMlShapeUri = "http://schemas.openxmlformats.org/drawingml/2006/main";
    private const string WordprocessingShapeUri = "http://schemas.microsoft.com/office/word/2010/wordprocessingShape";

    private const float FallbackAspectRatio = 0.6497696f;
    
    private string? _fallbackRelId;
    private string? _svgFallbackRelId;
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

    public WordProcessing.Drawing CreateSvgPanel(SvgPage svgPage, uint layer)
    {
        var relId = AddSvgToDocument(svgPage.Document);

        var svgBlip = new SVG.SVGBlip { Embed = relId };

        var svgExtension = new Drawing.BlipExtension { Uri = SvgExtension };
        svgExtension.AppendChild(svgBlip);

        var blipExtensionList = new Drawing.BlipExtensionList();
        blipExtensionList.AppendChild(svgExtension);

        // If the image can't be rendered, we show the "fallback" pixel
        var blip = new Drawing.Blip
        {
            Embed = GetSvgFallbackImageId()
        };
        blip.AppendChild(blipExtensionList);
        
        var width = svgPage.Width.ToEmusLong();
        var height = svgPage.Height.ToEmusLong();
        
        return new WordProcessing.Drawing
        {
            Anchor = new WordProcessingDrawing.Anchor(
                new WordProcessingDrawing.SimplePosition { X = 0, Y = 0 },
                new WordProcessingDrawing.HorizontalPosition(new WordProcessingDrawing.PositionOffset("0"))
                {
                    RelativeFrom = WordProcessingDrawing.HorizontalRelativePositionValues.Page
                },
                new WordProcessingDrawing.VerticalPosition(new WordProcessingDrawing.PositionOffset("0"))
                {
                    RelativeFrom = WordProcessingDrawing.VerticalRelativePositionValues.Page
                },
                new WordProcessingDrawing.Extent { Cx = width, Cy = height },
                new WordProcessingDrawing.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                new WordProcessingDrawing.WrapNone(),
                new WordProcessingDrawing.DocProperties { Id = _id++, Name = "Svg Image" },
                new WordProcessingDrawing.NonVisualGraphicFrameDrawingProperties(
                    new Drawing.GraphicFrameLocks { NoChangeAspect = true }
                ),
                new Drawing.Graphic(
                    new Drawing.GraphicData(
                        new DrawingPictures.Picture(
                            new DrawingPictures.NonVisualPictureProperties(
                                new DrawingPictures.NonVisualDrawingProperties { Id = _id++, Name = "Svg Image" },
                                new DrawingPictures.NonVisualPictureDrawingProperties()
                            ),
                            new DrawingPictures.BlipFill(new Drawing.Stretch { FillRectangle = new Drawing.FillRectangle() })
                            {
                                Blip = blip
                            },
                            new DrawingPictures.ShapeProperties(
                                new Drawing.PresetGeometry
                                {
                                    Preset = Drawing.ShapeTypeValues.Rectangle
                                }
                            )
                            {
                                Transform2D = new Drawing.Transform2D
                                {
                                    Offset = new Drawing.Offset { X = 0, Y = 0 },
                                    Extents = new Drawing.Extents { Cx = width, Cy = height }
                                }
                            }
                        )
                    )
                    {
                        Uri = DrawingMlPictureUri
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

    public WordProcessing.Drawing CreateHyperlinkClickBox(LinkPosition linkPosition, uint layer)
    {
        var hyperlinkRelId = hyperlinkRenderer.AddHyperlink(linkPosition.Uri);

        return new WordProcessing.Drawing
        {
            Anchor = new WordProcessingDrawing.Anchor(
                new WordProcessingDrawing.SimplePosition { X = 0, Y = 0 },
                new WordProcessingDrawing.HorizontalPosition(new WordProcessingDrawing.PositionOffset(linkPosition.X.ToEmusLong().ToString()))
                {
                    RelativeFrom = WordProcessingDrawing.HorizontalRelativePositionValues.Page
                },
                new WordProcessingDrawing.VerticalPosition(new WordProcessingDrawing.PositionOffset(linkPosition.Y.ToEmusLong().ToString()))
                {
                    RelativeFrom = WordProcessingDrawing.VerticalRelativePositionValues.Page
                },
                new WordProcessingDrawing.Extent
                {
                    Cx = linkPosition.Width.ToEmusLong(),
                    Cy = linkPosition.Height.ToEmusLong()
                },
                new WordProcessingDrawing.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                new WordProcessingDrawing.WrapNone(),
                new WordProcessingDrawing.DocProperties(
                    new Drawing.HyperlinkOnClick
                    {
                        Id = hyperlinkRelId
                    })
                {
                    Id = _id++,
                    Name = "Hyperlink Click Box"
                },
                new WordProcessingDrawing.NonVisualGraphicFrameDrawingProperties(
                    new Drawing.GraphicFrameLocks { NoChangeAspect = true }
                ),
                new Drawing.Graphic(
                    new Drawing.GraphicData(
                        new DrawingShape.WordprocessingShape(
                            new DrawingShape.NonVisualDrawingShapeProperties(
                                new DrawingShape.NonVisualDrawingProperties
                                {
                                    Id = _id++,
                                    Name = "Clickable Link Box"
                                },
                                new DrawingShape.NonVisualDrawingShapeProperties()
                            ),
                            new DrawingShape.ShapeProperties(
                                new Drawing.Transform2D(
                                    new Drawing.Offset
                                    {
                                        X = 0,
                                        Y = 0
                                    },
                                    new Drawing.Extents
                                    {
                                        Cx = linkPosition.Width.ToEmusLong(),
                                        Cy = linkPosition.Height.ToEmusLong()
                                    }
                                ),
                                new Drawing.PresetGeometry
                                {
                                    Preset = Drawing.ShapeTypeValues.Rectangle,
                                    AdjustValueList = new Drawing.AdjustValueList()
                                },
                                new Drawing.SolidFill(
                                    new Drawing.RgbColorModelHex(new Drawing.Alpha
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

    public WordProcessing.Drawing CreateMetadataDrawing(PrintPoint width)
    {
        var linkId = hyperlinkRenderer.AddHyperlink(new Uri("https://github.com/AnfriH/AquaVitae"));

        var widthEmus = width.ToEmusLong();
        var heightEmus = (FallbackAspectRatio * width).ToEmusLong();

        return new WordProcessing.Drawing
        {
            Inline = new WordProcessingDrawing.Inline
            {
                Extent = new WordProcessingDrawing.Extent
                {
                    Cx = widthEmus, 
                    Cy = heightEmus
                },
                EffectExtent = new WordProcessingDrawing.EffectExtent
                {
                    LeftEdge = 0L, 
                    TopEdge = 0L, 
                    RightEdge = 0L, 
                    BottomEdge = 0L
                },
                DocProperties = new WordProcessingDrawing.DocProperties
                {
                    Id = _id++,
                    Name = "Fallback image",
                    HyperlinkOnClick = new Drawing.HyperlinkOnClick
                    {
                        Id = linkId,
                        Tooltip = "Visit AquaVitae's Github"
                    }
                },
                NonVisualGraphicFrameDrawingProperties = new WordProcessingDrawing.NonVisualGraphicFrameDrawingProperties
                {
                    GraphicFrameLocks = new Drawing.GraphicFrameLocks { NoChangeAspect = true }
                },
                Graphic = new Drawing.Graphic
                {
                    GraphicData = new Drawing.GraphicData(
                        new DrawingPictures.Picture
                        {
                            NonVisualPictureProperties = new DrawingPictures.NonVisualPictureProperties
                            {
                                NonVisualDrawingProperties = new DrawingPictures.NonVisualDrawingProperties
                                {
                                    Id = _id++,
                                    Name = "Fallback image PNG"
                                },
                                NonVisualPictureDrawingProperties = new DrawingPictures.NonVisualPictureDrawingProperties()
                            },
                            BlipFill = new DrawingPictures.BlipFill(
                                new Drawing.Stretch
                                {
                                    FillRectangle = new Drawing.FillRectangle()
                                }    
                            )
                            {
                                Blip = new Drawing.Blip
                                {
                                    Embed = GetFallbackImageId(),
                                    CompressionState = Drawing.BlipCompressionValues.Screen
                                }
                            },
                            ShapeProperties = new DrawingPictures.ShapeProperties
                            {
                                Transform2D = new Drawing.Transform2D
                                {
                                    Offset = new Drawing.Offset { X = 0, Y = 0 },
                                    Extents = new Drawing.Extents { Cx = widthEmus, Cy = heightEmus }
                                }
                            }
                        }
                    )
                    {
                        Uri = DrawingMlShapeUri
                    }
                }
            }
        };
    }
    
    private string GetFallbackImageId()
    {
        if (_fallbackRelId != null) return _fallbackRelId;
        var imagePart = mainPart.AddImagePart(ImagePartType.Png);
        using (var stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Resources", "oops.png")))
        {
            imagePart.FeedData(stream);
        }

        _fallbackRelId = mainPart.GetIdOfPart(imagePart);
        return _fallbackRelId;
    }

    /// <summary>
    /// Loads a 1x1 transparent image to use as a fallback for SVG images.
    /// </summary>
    private string GetSvgFallbackImageId()
    {
        if (_svgFallbackRelId != null) return _svgFallbackRelId;
        var imagePart = mainPart.AddImagePart(ImagePartType.Png);
        using (var stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Resources", "1x1.png")))
        {
            imagePart.FeedData(stream);
        }

        _svgFallbackRelId = mainPart.GetIdOfPart(imagePart);
        return _svgFallbackRelId;
    }
}