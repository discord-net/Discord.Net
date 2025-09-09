namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXSourceReader
{
    public char this[int index]
        => Source.SourceSpan.Contains(index)
            ? Source[index]
            : CXLexer.NULL_CHAR;

    public bool IsEOF => Position >= Source.SourceSpan.End;

    public char Current => this[Position];

    public char Next => this[Position + 1];

    public char Previous => this[Position - 1];

    public bool IsInInterpolation => Source.IsAtInterpolation(Position);


    public int Position { get; set; }
    public CXSource Source { get; set; }

    public CXSourceReader(CXSource source)
    {
        Source = source;
        Position = source.SourceSpan.Start;
    }

    public void Advance(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            Position++;
        }
    }
}
