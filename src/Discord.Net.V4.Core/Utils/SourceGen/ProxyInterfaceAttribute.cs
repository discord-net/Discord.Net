namespace Discord;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ProxyInterfaceAttribute(params Type[] interfaces) : Attribute
{
    public readonly Type[] Interfaces = interfaces;
}
