
using Discord.EntityRelationships;

namespace Discord;

public interface IChannelRelationship : IChannelRelationship<IChannel>;
public interface IChannelRelationship<TChannel> : IRelationship<ulong, TChannel, ILoadableChannelEntitySource<TChannel>>
    where TChannel : class, IChannel
{
    ILoadableChannelEntitySource<TChannel> Channel { get; }

    ILoadableChannelEntitySource<TChannel> IRelationship<ulong, TChannel, ILoadableChannelEntitySource<TChannel>>.
        RelationshipLoadable => Channel;
}
