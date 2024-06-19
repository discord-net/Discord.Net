namespace Discord;

public interface IChannel :
    ISnowflakeEntity,
    IChannelActor
{
    ChannelType Type { get; }
}
