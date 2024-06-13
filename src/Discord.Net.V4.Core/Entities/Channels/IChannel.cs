namespace Discord;

public interface IChannel : IChannel<IChannel>;
public interface IChannel<out TChannel> :
    ISnowflakeEntity,
    IChannelActor<TChannel>
    where TChannel : IChannel<TChannel>
{
    /// <summary>
    ///     Gets the name of this channel.
    /// </summary>
    /// <returns>
    ///     A string containing the name of this channel.
    /// </returns>
    string Name { get; }
}
