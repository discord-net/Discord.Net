namespace Discord;

public interface IGuildChannelRelationship : IGuildChannelRelationship<IGuildChannel>;
public interface IGuildChannelRelationship<TChannel> :
    IChannelRelationship<TChannel, ILoadableGuildChannelActor<TChannel>>,
    IGuildRelationship
    where TChannel : class, IGuildChannel<TChannel>;

public interface IGuildChannelRelationship<TChannel, out TLoadable> :
    IChannelRelationship<TChannel, TLoadable>,
    IGuildRelationship
    where TChannel : class, IGuildChannel<TChannel>
    where TLoadable : ILoadableGuildChannelActor<TChannel>;
