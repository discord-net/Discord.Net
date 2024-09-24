namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class DiscriminatedUnionRootTypeAttribute(string? property = null) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.
