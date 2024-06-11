namespace Discord;

public interface ILoadableMessageChannelEntitySource<TMessageChannel> :
    IMessageChannelEntitySource<TMessageChannel>,
    ILoadableChannelEntitySource<TMessageChannel>
    where TMessageChannel : class, IMessageChannel;

public interface IMessageChannelEntitySource<out TMessageChannel> :
    IChannelEntitySource<TMessageChannel>
    where TMessageChannel : IMessageChannel
{
    IRootEntitySource<ILoadableEntity<ulong, IMessage>, ulong, IMessage> Messages { get; }
    ILoadableEntity<ulong, IMessage> Message(ulong id) => Messages[id];
}
