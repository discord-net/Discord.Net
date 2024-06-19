
namespace Discord;

public interface IChannelRelationship : IChannelRelationship<ILoadableChannelActor>;

public interface IChannelRelationship<out TLoadable> :
    IRelationship<ulong, IChannel, TLoadable>
    where TLoadable : ILoadableChannelActor
{
    TLoadable Channel { get; }

    TLoadable IRelationship<ulong, IChannel, TLoadable>.
        RelationshipLoadable => Channel;
}
