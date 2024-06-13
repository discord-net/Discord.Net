namespace Discord;

public interface ILoadableChannelActor<TChannel> :
    IChannelActor<TChannel>,
    ILoadableEntity<ulong, TChannel>
    where TChannel : class, IChannel<TChannel>;

public interface IChannelActor<out TChannel> :
    IActor<ulong, TChannel>
    where TChannel : IChannel<TChannel>;
