using System.Runtime.CompilerServices;
using System.Text;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using AquaVitae.Render.Vector;

namespace AquaVitae;

public static class Program
{
    public static async Task Main()
    {
        var documentLayout = new DocumentLayout();

        var page1 = new PageLayout(PageSizes.A4);
        documentLayout.AddPage(page1);

        var txtBox = new TextBoxLayout(0, 0, PageSizes.A4.Width / 3, PageSizes.A4.Height)
        {
            FillColor = Color.FromString("#075700")
        };
        
        page1.AddPageElement(txtBox);
        
        var pdf = new VectorDocumentRenderer().RenderAsPdf(documentLayout);
        
        var stream = new MemoryStream();
        pdf.Write(stream);

        await using var file = File.Open($"{GetProjectDirectory()}/output.pdf", FileMode.Create, FileAccess.ReadWrite);
        stream.Position = 0;
        await stream.CopyToAsync(file);
    }
    
    public static string GetProjectDirectory([CallerFilePath] string sourceFilePath = "")
    {
        // Returns the directory containing this specific C# source file
        return Path.GetDirectoryName(sourceFilePath)!; 
    }
}