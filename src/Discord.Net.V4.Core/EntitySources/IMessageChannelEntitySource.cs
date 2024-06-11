namespace Discord;

public interface IMessageChannelEntitySource<TChannel> : IClientProvider, ILoadableEntity<ulong, TChannel>
    where TChannel : class, IMessageChannel
{
    IRootEntitySource<ILoadableEntity<ulong, IMessage>, ulong, IMessage> Messages { get; }
    ILoadableEntity<ulong, IMessage> Message(ulong id) => Messages[id];
}
