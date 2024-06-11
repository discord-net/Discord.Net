namespace Discord;

public interface ILoadableChannelEntitySource<TChannel> :
    IChannelEntitySource<TChannel>,
    ILoadableEntity<ulong, TChannel>
    where TChannel : class, IChannel;

public interface IChannelEntitySource<out TChannel> :
    IEntitySource<ulong, TChannel>
    where TChannel : IChannel;
