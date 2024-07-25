namespace Discord;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class ExtendInterfaceDefaultsAttribute(params Type[] toExtend) : Attribute
{
    public readonly Type[] ToExtend = toExtend;

    public bool ExtendAll { get; set; }
}

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class TemplateExtensionAttribute : Attribute
{
    public Type? TakesPrecedenceOver { get; set; }
    public bool FirstPartyOnly { get; set; }
}
