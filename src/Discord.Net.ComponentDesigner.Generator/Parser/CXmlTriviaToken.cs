using System;

namespace Discord.ComponentDesigner.Generator.Parser;

public enum CXmlTokenKind
{
    ElementStart,
    ElementEnd,
    Equals
}

public readonly record struct CXmlToken(
    CXmlTokenKind Kind,
    int LeadingTriviaLength,
    int TrailingTriviaLength,
    int Width
);

public readonly record struct CXmlTriviaToken(
    TriviaKind Kind,
    SourceSpan Span,
    string Value
);

public readonly record struct TriviaTokenSpan(
    int Start,
    int Count
);

public enum TriviaKind
{
    CommentStart,
    CommentText,
    CommentEnd,
    Newline,
    Whitespace
}
