using System.Xml;
using AquaVitae.Layouts;
using AquaVitae.Render.Vector;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Office2019.Drawing.SVG;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Anchor = DocumentFormat.OpenXml.Drawing.Wordprocessing.Anchor;
using BlipFill = DocumentFormat.OpenXml.Drawing.Pictures.BlipFill;
using NonVisualDrawingProperties = DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties;
using NonVisualPictureDrawingProperties = DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties;
using NonVisualPictureProperties = DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties;
using Picture = DocumentFormat.OpenXml.Drawing.Pictures.Picture;
using ShapeProperties = DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties;

namespace AquaVitae.Render.Docx;

public sealed class DocxSvgRenderer(MainDocumentPart mainPart)
{
    private const string SvgExtension = "{96DAC541-7B7A-43D3-8B79-37D633B846F1}";
    private const string DrawingMlUri = "http://schemas.openxmlformats.org/drawingml/2006/picture";
    private uint _id;
    
    public Drawing RenderSvg(SvgPage svgPage)
    {
        var relId = AddSvgToDocument(svgPage.Svg);
        
        var svgBlip = new SVGBlip { Embed = relId };

        var svgExtension = new BlipExtension { Uri = SvgExtension };
        svgExtension.AppendChild(svgBlip);

        var blipExtensionList = new BlipExtensionList();
        blipExtensionList.AppendChild(svgExtension);

        var blip = new Blip();
        blip.AppendChild(blipExtensionList);

        var width = svgPage.Width;
        var height = svgPage.Height;
        
        var drawing = new Drawing
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
                new Extent { Cx = width.ToEmusInt(), Cy = height.ToEmusInt() },
                new EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                new WrapNone(),
                new DocProperties { Id = _id++, Name = "Svg Image"},
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
                                    Extents = new Extents{ Cx = width.ToEmusInt(), Cy = height.ToEmusInt() }
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
                RelativeHeight = uint.MaxValue,
                BehindDoc = false,
                Locked = false,
                LayoutInCell = true,
                AllowOverlap = true
            }
        };

        return drawing;
    }

    private string AddSvgToDocument(XmlDocument svg)
    {
        var imagePart = mainPart.AddImagePart(ImagePartType.Svg);
        var stream = new MemoryStream();
        svg.Save(stream);
        stream.Seek(0, SeekOrigin.Begin);
        imagePart.FeedData(stream);

        return mainPart.GetIdOfPart(imagePart);
    }
}