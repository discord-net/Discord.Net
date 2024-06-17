
namespace Discord;

public interface IChannelRelationship : IChannelRelationship<IChannel>;

public interface IChannelRelationship<TChannel> :
    IChannelRelationship<TChannel, ILoadableChannelActor<TChannel>>
    where TChannel : class, IChannel;

public interface IChannelRelationship<TChannel, out TLoadable> :
    IRelationship<ulong, TChannel, TLoadable>
    where TChannel : class, IChannel
    where TLoadable : ILoadableChannelActor<TChannel>
{
    TLoadable Channel { get; }

    TLoadable IRelationship<ulong, TChannel, TLoadable>.
        RelationshipLoadable => Channel;
}
