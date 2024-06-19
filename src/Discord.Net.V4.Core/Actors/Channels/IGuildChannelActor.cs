using Discord.Invites;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildChannelActor :
    IGuildChannelActor,
    ILoadableEntity<ulong, IGuildChannel>;

public interface IGuildChannelActor :
    IChannelActor,
    IGuildRelationship,
    IActor<ulong, IGuildChannel>,
    IDeletable<ulong, IGuildChannelActor>,
    IModifiable<ulong, IGuildChannelActor, ModifyGuildChannelProperties, ModifyGuildChannelParams>
{
    ILoadableRootActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites { get; }

    async Task<IInvite> CreateInviteAsync(
        CreateChannelInviteProperties args,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.CreateChannelInvite(Id, args.ToApiModel()),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return Client.CreateEntity(model);
    }

    static BasicApiRoute IDeletable<ulong, IGuildChannelActor>.DeleteRoute(IPathable pathable, ulong id)
        => Routes.DeleteChannel(id);

    static ApiBodyRoute<ModifyGuildChannelParams> IModifiable<ulong, IGuildChannelActor, ModifyGuildChannelProperties, ModifyGuildChannelParams>.ModifyRoute(IPathable path, ulong id,
        ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
