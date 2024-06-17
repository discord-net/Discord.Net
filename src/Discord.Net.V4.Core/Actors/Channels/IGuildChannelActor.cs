using Discord.Invites;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildChannelActor<TGuildChannel> :
    IGuildChannelActor,
    ILoadableChannelActor<TGuildChannel>
    where TGuildChannel : class, IGuildChannel;

public interface IGuildChannelActor :
    IChannelActor,
    IGuildRelationship,
    IActor<ulong, IGuildChannel>,
    IDeletable<ulong, IGuildChannelActor>,
    IModifiable<ulong, IGuildChannelActor, ModifyGuildChannelProperties, ModifyGuildChannelParams>
{
    ILoadableRootActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites { get; }

    Task<IInvite> CreateInviteAsync(
        CreateChannelInviteProperties args,
        RequestOptions? options = null,
        CancellationToken token = default);

    static BasicApiRoute IDeletable<ulong, IGuildChannelActor>.DeleteRoute(IPathable pathable, ulong id)
        => Routes.DeleteChannel(id);

    static ApiBodyRoute<ModifyGuildChannelParams> IModifiable<ulong, IGuildChannelActor, ModifyGuildChannelProperties, ModifyGuildChannelParams>.ModifyRoute(IPathable path, ulong id,
        ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
