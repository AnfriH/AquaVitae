using System.Diagnostics;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorParagraphRenderer(DocumentLayout documentLayout)
{
    private float _cursorX = 0;
    private float _cursorY = 0;
    
    private float _x = 0;
    private float _y = 0;
    
    private float _width = 30;
    private float _height = 20;
    
    public void WriteParagraph(ParagraphLayout paragraphLayout)
    {
        // TODO: Setup some paragraph metadata surrounding the paragraph spacing, etc
        var paraStyleId = paragraphLayout.Style;
        var paraStyle = documentLayout.ParagraphStyles[paraStyleId.Value!];
        
        foreach (var runLayout in paragraphLayout.Runs)
        {
            var runStyleId = runLayout.Style;
            var runStyle = documentLayout.RunStyles[runStyleId.Value!];
            
            WriteRun(runLayout.Text, paraStyle, runStyle);
        }
    }

    private void WriteRun(string text, ParagraphStyle paraStyle, RunStyle runStyle)
    {
        var type = TokenType.Newline;
        var start = 0;
        int end;
        
        for (end = 0; end < text.Length; end++)
        {
            var ch = text[end];
            TokenType chType;
            
            switch (ch)
            {
                case '\n':
                {
                    chType = TokenType.Newline;
                    WriteToken();
                    break;
                }
                case '\r':
                {
                    chType = TokenType.Newline;
                    WriteToken();
                    
                    // "\r\n" is treated as a single newline, rather than multiple.
                    if (end + 1 < text.Length && text[end + 1] == '\n') end++;
                    break;
                }
                default:
                {
                    chType = char.IsWhiteSpace(ch) ? TokenType.Whitespace : TokenType.Word;
                    if (type == chType) continue;
                    WriteToken();
                    break;
                }
            }
            
            start = end;
            type = chType;
        }

        // Write anything remaining
        WriteToken();
        return;

        void WriteToken()
        {
            switch (type)
            {
                case TokenType.Newline:
                    // As we start with the type set to Newline, we skip the first encountered newline token
                    if (end != 0) WriteNewline(paraStyle, runStyle);
                    break;
                case TokenType.Word:
                    WriteWord(text.AsSpan(start, end - start), paraStyle, runStyle);
                    break;
                case TokenType.Whitespace:
                    WriteWhitespace(text.AsSpan(start, end - start), paraStyle, runStyle);
                    break;
                default:
                    throw new UnreachableException();
            }
        }
    }

    private enum TokenType
    {
        Newline,
        Word,
        Whitespace
    }

    private void WriteNewline(ParagraphStyle paraStyle, RunStyle runStyle)
    {
        Console.WriteLine("Writing newline");
    }

    private void WriteWhitespace(ReadOnlySpan<char> text, ParagraphStyle paraStyle, RunStyle runStyle)
    {
        Console.WriteLine($"Writing whitespace '{new string(text)}'");
    }

    private void WriteWord(ReadOnlySpan<char> text, ParagraphStyle paraStyle, RunStyle runStyle)
    {
        Console.WriteLine($"Writing word '{new string(text)}'");
    }
}

