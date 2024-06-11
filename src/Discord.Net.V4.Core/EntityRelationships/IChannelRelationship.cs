
using Discord.EntityRelationships;

namespace Discord;

public interface IChannelRelationship : IChannelRelationship<IChannel>;
public interface IChannelRelationship<TChannel> : ILoadableRelationship<ulong, TChannel>
    where TChannel : class, IChannel
{
    ILoadableEntity<ulong, TChannel> Channel { get; }

    ILoadableEntity<ulong, TChannel> IRelationship<ulong, TChannel, ILoadableEntity<ulong, TChannel>>.
        RelationshipLoadable => Channel;
}
