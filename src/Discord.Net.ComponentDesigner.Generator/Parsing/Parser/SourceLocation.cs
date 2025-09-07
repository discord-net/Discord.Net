namespace Discord.ComponentDesignerGenerator.Parser;

public readonly record struct SourceLocation(
    int Line,
    int Column,
    int Offset
)
{
    public static implicit operator SourceLocation((int, int, int) tuple) => new(tuple.Item1, tuple.Item2, tuple.Item3);
}
