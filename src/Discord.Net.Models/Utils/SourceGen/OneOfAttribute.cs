namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.GenericParameter)]
internal sealed class OneOfAttribute(params Type[] types) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
