namespace Discord;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public sealed class SourceOfTruthAttribute : Attribute
{
    public bool Force { get; init; }
}
