using Microsoft.CodeAnalysis.Text;

namespace Discord.ComponentDesignerGenerator.Parser;

public readonly record struct CXToken(
    CXTokenKind Kind,
    TextSpan Span,
    int LeadingTriviaLength,
    int TrailingTriviaLength,
    CXTokenFlags Flags
)
{
    public int AbsoluteStart => Span.Start - LeadingTriviaLength;
    public int AbsoluteEnd => Span.End + TrailingTriviaLength;

    public int AbsoluteWidth =>  AbsoluteEnd - AbsoluteStart;

    public TextSpan FullSpan => new(AbsoluteStart, AbsoluteWidth);
}
