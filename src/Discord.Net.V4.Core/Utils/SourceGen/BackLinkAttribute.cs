namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class BackLinkAttribute(string property) : Attribute;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal sealed class BackLinkAttribute<TSource>(string? methodName = null) : Attribute
    where TSource : class, IPathable;

#pragma warning restore CS9113 // Parameter is unread.
