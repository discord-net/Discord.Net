namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = true)]
internal sealed class NotAttribute(string name) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.
