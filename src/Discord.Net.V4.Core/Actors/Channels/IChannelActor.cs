namespace Discord;

public interface ILoadableChannelActor<TChannel> :
    IChannelActor,
    ILoadableEntity<ulong, TChannel>
    where TChannel : class, IChannel;

public interface IChannelActor :
    IActor<ulong, IChannel>;
