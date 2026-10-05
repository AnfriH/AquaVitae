using AquaVitae.Data;
using AquaVitae.Outputs;
using AquaVitae.Render.Docx;
using AquaVitae.Render.Vector;
using Tomlyn;

namespace AquaVitae;

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length < 2) throw new ArgumentException("Not enough arguments provided, requires <cv_toml> <docx_output>");
        
        CvData data;
        await using (var inFile = File.OpenRead(args[0]))
        {
            data = TomlSerializer.Deserialize(inFile, CvDataContext.Default.CvData) ?? throw new Exception();
        }
        
        var documentLayout = new CvBuilder(data).Build();
        
        var svg = new VectorDocumentRenderer().RenderAsSvg(documentLayout);
        var docxRenderer = new DocxDocumentRenderer(new DocxRendererSettings { IncludeTextColor = false });
        
        
        await using (var outDoc = File.Open(args[1], FileMode.Create, FileAccess.ReadWrite))
        {
            docxRenderer.RenderDocument(documentLayout, svg, outDoc);
        }
    }
}