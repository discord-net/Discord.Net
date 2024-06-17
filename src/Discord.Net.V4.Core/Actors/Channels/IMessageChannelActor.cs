namespace Discord;

public interface ILoadableMessageChannelActor<TMessageChannel> :
    IMessageChannelActor,
    ILoadableChannelActor<TMessageChannel>
    where TMessageChannel : class, IMessageChannel;

public interface IMessageChannelActor :
    IChannelActor,
    IActor<ulong, IMessageChannel>
{
    IRootActor<ILoadableMessageActor<IMessage>, ulong, IMessage> Messages { get; }
    ILoadableMessageActor<IMessage> Message(ulong id) => Messages[id];
}
