using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;

public sealed partial class RestLoadableBanActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestBanActor(client, guildId, id),
    ILoadableGuildBanActor
{
    [ProxyInterface(typeof(ILoadableEntity<IBan>))]
    internal RestLoadable<ulong, RestBan, IBan, Ban> Loadable { get; } =
        RestLoadable<ulong, RestBan, IBan, Ban>.FromContextConstructable<RestBan, ulong>(
            client,
            id,
            Routes.GetGuildBan,
            guildId
        );
}

[ExtendInterfaceDefaults(typeof(IGuildBanActor))]
public partial class RestBanActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestActor<ulong, RestBan>(client, id),
    IGuildBanActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guildId);

    public RestLoadableUserActor User { get; } = new(client, id);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
    ILoadableUserActor IUserRelationship.User => User;
}

public sealed partial class RestBan(DiscordRestClient client, ulong guildId, IBanModel model, RestBanActor? actor = null) :
    RestEntity<ulong>(client, model.UserId),
    IBan,
    IContextConstructable<RestBan, IBanModel, ulong, DiscordRestClient>
{
    [ProxyInterface(
        typeof(IGuildBanActor),
        typeof(IUserRelationship),
        typeof(IGuildRelationship)
    )]
    internal RestBanActor Actor { get; } = actor ?? new(client, guildId, model.UserId);
    internal IBanModel Model { get; } = model;

    public static RestBan Construct(DiscordRestClient client, IBanModel model, ulong guildId)
        => new(client, guildId, model);

    public string? Reason => Model.Reason;
}
