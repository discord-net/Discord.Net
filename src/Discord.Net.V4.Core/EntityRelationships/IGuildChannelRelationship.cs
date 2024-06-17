namespace Discord;

public interface IGuildChannelRelationship : IGuildChannelRelationship<IGuildChannel>;
public interface IGuildChannelRelationship<TChannel> :
    IChannelRelationship<TChannel, ILoadableGuildChannelActor<TChannel>>,
    IGuildRelationship
    where TChannel : class, IGuildChannel;

public interface IGuildChannelRelationship<TChannel, out TLoadable> :
    IChannelRelationship<TChannel, TLoadable>
    where TChannel : class, IGuildChannel
    where TLoadable : ILoadableGuildChannelActor<TChannel>;
