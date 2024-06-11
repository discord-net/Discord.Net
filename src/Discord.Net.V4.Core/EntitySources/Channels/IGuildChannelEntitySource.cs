using Discord.EntityRelationships;

namespace Discord;

public interface ILoadableGuildChannelEntitySource<TGuildChannel> :
    ILoadableChannelEntitySource<TGuildChannel>,
    IGuildChannelEntitySource<TGuildChannel>
    where TGuildChannel : class, IGuildChannel;

public interface IGuildChannelEntitySource<out TGuildChannel> :
    IChannelEntitySource<TGuildChannel>,
    IGuildRelationship
    where TGuildChannel : IGuildChannel;
