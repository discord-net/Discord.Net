namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Method)]
public class OnVertexAttribute(string? methodName = null) : Attribute;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class OnVertexAttribute<TTarget>(string? methodName = null) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
