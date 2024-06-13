using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildChannelActor<TGuildChannel> :
    ILoadableChannelActor<TGuildChannel>,
    IGuildChannelActor<TGuildChannel>
    where TGuildChannel : class, IGuildChannel<TGuildChannel>;

public interface IGuildChannelActor<out TGuildChannel> :
    IChannelActor<TGuildChannel>,
    IGuildRelationship,
    IDeletable<ulong, IGuildChannelActor<TGuildChannel>>,
    IModifiable<ulong, IGuildChannelActor<TGuildChannel>, ModifyGuildChannelProperties, ModifyGuildChannelParams>
    where TGuildChannel : IGuildChannel<TGuildChannel>
{
    static BasicApiRoute IDeletable<ulong, IGuildChannelActor<TGuildChannel>>.DeleteRoute(IPathable pathable, ulong id)
        => Routes.DeleteChannel(id);

    static ApiBodyRoute<ModifyGuildChannelParams> IModifiable<ulong, IGuildChannelActor<TGuildChannel>, ModifyGuildChannelProperties, ModifyGuildChannelParams>.ModifyRoute(IPathable path, ulong id,
        ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
