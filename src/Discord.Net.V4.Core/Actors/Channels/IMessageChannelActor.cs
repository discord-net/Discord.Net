namespace Discord;

public interface ILoadableMessageChannelActor<TMessageChannel> :
    IMessageChannelActor<TMessageChannel>,
    ILoadableChannelActor<TMessageChannel>
    where TMessageChannel : class, IMessageChannel<TMessageChannel>;

public interface IMessageChannelActor<out TMessageChannel> :
    IChannelActor<TMessageChannel>
    where TMessageChannel : IMessageChannel<TMessageChannel>
{
    IRootActor<ILoadableEntity<ulong, IMessage>, ulong, IMessage> Messages { get; }
    ILoadableEntity<ulong, IMessage> Message(ulong id) => Messages[id];
}
