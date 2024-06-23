namespace Discord;

public interface ILoadableMessageChannelActor :
    IMessageChannelActor,
    ILoadableEntity<ulong, IMessageChannel>;

public interface IMessageChannelActor :
    IChannelActor,
    IActor<ulong, IMessageChannel>
{
    IIndexableActor<ILoadableMessageActor, ulong, IMessage> Messages { get; }
    ILoadableMessageActor Message(ulong id) => Messages[id];
}
