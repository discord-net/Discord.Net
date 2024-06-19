namespace Discord;

public interface ILoadableChannelActor :
    IChannelActor,
    ILoadableEntity<ulong, IChannel>;

public interface IChannelActor :
    IActor<ulong, IChannel>;
