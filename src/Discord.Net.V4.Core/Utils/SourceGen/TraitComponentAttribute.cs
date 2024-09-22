namespace Discord;

internal class TraitComponentAttribute : Attribute
{
    public Type? Parent { get; set; }
}

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
internal sealed class TraitLinkExtendsAttribute(string name, Type type) : TraitComponentAttribute
{
    public string? Getter { get; set; }
}
#pragma warning restore CS9113 // Parameter is unread.
