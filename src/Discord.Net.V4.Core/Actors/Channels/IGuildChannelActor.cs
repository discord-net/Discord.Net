using Discord.Invites;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, IGuildChannelActor, ModifyGuildChannelProperties, ModifyGuildChannelParams, IGuildChannel,
        IGuildChannelModel>;

public interface ILoadableGuildChannelActor :
    IGuildChannelActor,
    ILoadableEntity<ulong, IGuildChannel>;

public interface IGuildChannelActor :
    IChannelActor,
    IGuildRelationship,
    IActor<ulong, IGuildChannel>,
    IDeletable<ulong, IGuildChannelActor>,
    IModifiable
{
    IEnumerableIndexableActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites { get; }

    static IApiRoute IDeletable<ulong, IGuildChannelActor>.DeleteRoute(IPathable pathable, ulong id)
        => Routes.DeleteChannel(id);

    static IApiInOutRoute<ModifyGuildChannelParams, IEntityModel> IModifiable.ModifyRoute(
            IPathable path, ulong id,
            ModifyGuildChannelParams args
    ) => Routes.ModifyChannel(id, args);

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
}
