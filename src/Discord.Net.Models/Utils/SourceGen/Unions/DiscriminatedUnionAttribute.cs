namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Property)]
public sealed class DiscriminatedUnionAttribute(string name) : Attribute;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class DiscriminatedUnionEntryAttribute<T>(params object[] value) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
