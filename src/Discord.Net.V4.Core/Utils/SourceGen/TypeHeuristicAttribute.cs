namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Property | AttributeTargets.Field)]
internal sealed class TypeHeuristicAttribute(string? source = null) : Attribute
{
    public int GenericPosition { get; init; }
}

[AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Property | AttributeTargets.Field)]
internal sealed class TypeHeuristicAttribute<T>(string? source = null) : Attribute
{
    public int GenericPosition { get; init; }
}

#pragma warning restore CS9113 // Parameter is unread.
