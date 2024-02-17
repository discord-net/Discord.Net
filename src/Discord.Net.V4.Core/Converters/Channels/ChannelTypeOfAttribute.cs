namespace Discord.Converters;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ChannelTypeOfAttribute : Attribute
{
    public readonly ChannelType Type;

    public ChannelTypeOfAttribute(ChannelType type)
    {
        Type = type;
    }
}
