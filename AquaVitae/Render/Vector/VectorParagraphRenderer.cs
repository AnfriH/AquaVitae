using System.Diagnostics;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Styles;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorParagraphRenderer(
    double x,
    double y,
    double width,
    VectorStyleRenderer styleRenderer,
    Graphics graphics
)
{
    private readonly Graphics _graphics = graphics;
    private readonly LineBuilder _lineBuilder = new(width);

    private double _x = x;
    private double _y = y;

    public void WriteParagraph(ParagraphLayout paragraphLayout)
    {
        var paraStyle = styleRenderer.GetParagraphStyle(paragraphLayout.Style);
        
        foreach (var runLayout in paragraphLayout.Runs)
        {
            var font = styleRenderer.GetFont(runLayout.Style);
            
            WriteRun(runLayout.Text, paraStyle, font);
        }
        
        WriteParagraphEnd();
    }

    private void WriteRun(string text, ParagraphStyle paraStyle, Font font)
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
                    if (end != 0) WriteNewline(paraStyle, font);
                    break;
                case TokenType.Word:
                    WriteWord(text.Substring(start, end - start), paraStyle, font);
                    break;
                case TokenType.Whitespace:
                    WriteWhitespace(text.Substring(start, end - start), font);
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

    private void WriteNewline(ParagraphStyle paraStyle, Font font)
    {
        WriteLine();
    }

    private void WriteWhitespace(string text, Font font)
    {
        _lineBuilder.AddWhitespace(text, font, font.MeasureTextAdvanced(text).AdvanceWidth);
    }
    
    private void WriteWord(string text, ParagraphStyle paraStyle, Font font)
    {
        var advanceWidth = font.MeasureTextAdvanced(text).AdvanceWidth;

        if (advanceWidth > _lineBuilder.Width) throw new NotImplementedException($"Text was too big: '{text}' is {advanceWidth} > {_lineBuilder.Width}");
        
        if (!_lineBuilder.AddWord(text, font, advanceWidth))
        {
            WriteLine();
            _lineBuilder.AddWord(text, font, advanceWidth);
        }
    }

    private void WriteParagraphEnd()
    {
        WriteLine();
    }

    private void WriteLine()
    {
        var phrases = _lineBuilder.GetPhrases();
        Console.WriteLine(string.Concat(phrases.Select(ph => ph.Text)));
        _lineBuilder.Clear();
    }
}

internal readonly record struct Phrase(string Text, Font Font, double AdvanceWidth);

internal sealed class LineBuilder(double width)
{
    public double Width => width;

    private double _totalWidth;
    private double _phrasePiecesWidth;
    private Font? _phrasePiecesFont;                                                                                                                                                                                                                                                                                                                                                                                                                                               
    
    private readonly List<string> _whitespacePhrasePieces = [];
    private readonly List<Phrase> _whitespacePhrases = [];
    
    private readonly List<string> _phrasePieces = [];
    private readonly List<Phrase> _phrases = [];

    public void AddWhitespace(string text, Font font, double advanceWidth)
    {
        // Like LibreOffice, we do not check if there is room for a whitespace until a word is added
        if (_phrasePiecesFont == null) _phrasePiecesFont = font;
        else if (_phrasePiecesFont != font)
        {
            _whitespacePhrases.Add(new Phrase(
                string.Concat(_whitespacePhrasePieces),
                _phrasePiecesFont,
                _phrasePiecesWidth
            ));
            _whitespacePhrasePieces.Clear();
            _phrasePiecesFont = font;
            _phrasePiecesWidth = 0;
        }
        
        _whitespacePhrasePieces.Add(text);
        _phrasePiecesWidth += advanceWidth;
        _totalWidth += advanceWidth;
    }

    public bool AddWord(string text, Font font, double advanceWidth)
    {
        // If there's no room left on this line, we return false to indicate that the line is full
        if (advanceWidth + _totalWidth > Width) return false;

        // Flush all pending phrases
        if (_whitespacePhrases.Count > 0)
        {
            _phrases.AddRange(_whitespacePhrases);
            _whitespacePhrases.Clear();
        }
        
        // Flush all pending phrase pieces
        if (_whitespacePhrasePieces.Count > 0)
        {
            _phrasePieces.AddRange(_whitespacePhrasePieces);
            _whitespacePhrasePieces.Clear();
        }
        
        if (_phrasePiecesFont == null) _phrasePiecesFont = font;
        else if (_phrasePiecesFont != font)
        {
            _phrases.Add(new Phrase(
                string.Concat(_phrasePieces),
                font,
                _phrasePiecesWidth
            ));
            _phrasePieces.Clear();
            _phrasePiecesFont = font;
            _phrasePiecesWidth = 0;
        }
        
        _phrasePieces.Add(text);
        _phrasePiecesWidth += advanceWidth;
        _totalWidth += advanceWidth;

        return true;
    }

    public IReadOnlyList<Phrase> GetPhrases()
    {
        if (_phrasePieces.Count > 0)
        {
            _phrases.Add(new Phrase(
                string.Concat(_phrasePieces),
                _phrasePiecesFont!,
                _phrasePiecesWidth
            ));
            _phrasePieces.Clear();
            _phrasePiecesFont = null;
            _phrasePiecesWidth = 0;
        }
        
        return _phrases;
    }

    public void Clear()
    {
        _phrases.Clear();
        _whitespacePhrases.Clear();
        _phrasePieces.Clear();
        _whitespacePhrasePieces.Clear();
        _phrasePiecesFont = null;
        _phrasePiecesWidth = 0;
        _totalWidth = 0;
    }
}

