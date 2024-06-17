namespace Discord;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class ExtendInterfaceDefaultsAttribute(params Type[] toExtend) : Attribute
{
    public readonly Type[] ToExtend = toExtend;
}
