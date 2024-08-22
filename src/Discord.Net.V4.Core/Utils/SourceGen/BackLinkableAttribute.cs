namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
internal sealed class BackLinkableAttribute(string? backlinkName = null) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
