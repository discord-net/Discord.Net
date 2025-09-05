namespace Discord.ComponentDesigner.Generator.Parser;

public readonly record struct SourceSpan(
    SourceLocation Start,
    SourceLocation End
)
{
    public static implicit operator SourceSpan((SourceLocation, SourceLocation) tuple) => new(tuple.Item1, tuple.Item2);

    public int Length => End.Offset - Start.Offset;

    public int LineDelta => End.Line - Start.Line;
    public int ColumnDelta => End.Column - Start.Column;

}
