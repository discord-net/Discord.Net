using Microsoft.CodeAnalysis.Text;
using System.Net.Mime;

namespace Discord.CX.Parser;

public abstract class TextLineCollection
{
    public abstract int Count { get; }
    public abstract TextLine this[int index] { get; }

    public abstract int IndexOf(int position);

    public virtual TextLine GetLineFromPosition(int position) => this[IndexOf(position)];

    public virtual SourceLocation GetSourceLocation(int position)
    {
        var line = GetLineFromPosition(position);
        return new(line.LineNumber, position - line.Start, position);
    }
}

public readonly record struct TextLine(
    CXSourceText Source,
    int Start,
    int EndIncludingBreaks
)
{
    public int LineNumber => Source.Lines.IndexOf(Start);

    public int End => EndIncludingBreaks - LineBreakLength;

    public TextSpan Span => TextSpan.FromBounds(Start, End);
    public TextSpan SpanIncludingBreaks => TextSpan.FromBounds(Start, End);


    private int LineBreakLength
    {
        get
        {
            var ch = Source[EndIncludingBreaks - 1];

            if (ch is '\n')
            {
                if (EndIncludingBreaks > 1 && Source[EndIncludingBreaks - 2] is '\r')
                    return 2;

                return 1;
            }

            if (ch.IsNewline()) return 1;

            return 0;
        }
    }
}
