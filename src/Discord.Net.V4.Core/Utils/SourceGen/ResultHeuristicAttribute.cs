namespace Discord;

[AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Property | AttributeTargets.Field)]
internal sealed class TypeHeuristicAttribute(string source) : Attribute
{
    private readonly string _source = source;

    public int GenericPosition { get; init; }
}
