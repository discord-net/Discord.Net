namespace Discord;

public interface ILoadableMessageChannelActor :
    IMessageChannelActor,
    ILoadableChannelActor;

public interface IMessageChannelActor :
    IChannelActor,
    IActor<ulong, IMessageChannel>
{
    IRootActor<ILoadableMessageActor<IMessage>, ulong, IMessage> Messages { get; }
    ILoadableMessageActor<IMessage> Message(ulong id) => Messages[id];
}
