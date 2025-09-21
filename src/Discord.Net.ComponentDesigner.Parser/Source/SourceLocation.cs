namespace Discord.CX.Parser;

public readonly record struct SourceLocation(
    int Line,
    int Column,
    int Position
);
