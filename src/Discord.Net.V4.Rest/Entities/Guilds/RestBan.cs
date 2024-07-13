using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;

public sealed partial class RestLoadableBanActor(
    DiscordRestClient client,
    GuildIdentity guild,
    BanIdentity ban
):
    RestBanActor(client, guild, ban),
    ILoadableGuildBanActor
{
    [ProxyInterface(typeof(ILoadableEntity<IBan>))]
    internal RestLoadable<ulong, RestBan, IBan, IBanModel> Loadable { get; } =
        RestLoadable<ulong, RestBan, IBan, IBanModel>.FromContextConstructable<RestBan, GuildIdentity>(
            client,
            ban,
            (guild, id) => Routes.GetGuildBan(guild.Id, id),
            guild
        );
}

[ExtendInterfaceDefaults(typeof(IGuildBanActor))]
public partial class RestBanActor(
    DiscordRestClient client,
    GuildIdentity guild,
    BanIdentity ban
) :
    RestActor<ulong, RestBan, BanIdentity>(client, ban),
    IGuildBanActor
{
    [SourceOfTruth]
    public RestLoadableGuildActor Guild { get; } = new(client, guild);

    [SourceOfTruth]
    public RestLoadableUserActor User { get; } = new(client, UserIdentity.Of(ban.Id));
}

public sealed partial class RestBan :
    RestEntity<ulong>,
    IBan,
    IContextConstructable<RestBan, IBanModel, GuildIdentity, DiscordRestClient>
{
    [ProxyInterface(
        typeof(IGuildBanActor),
        typeof(IUserRelationship),
        typeof(IGuildRelationship)
    )]
    internal RestBanActor Actor { get; }
    internal IBanModel Model { get; private set; }

    public RestBan(DiscordRestClient client,
        GuildIdentity guild,
        IBanModel model,
        RestBanActor? actor = null) : base(client, model.UserId)
    {
        Actor = actor ?? new(client, guild, BanIdentity.Of(this));
        Model = model;
    }

    public static RestBan Construct(DiscordRestClient client, IBanModel model, GuildIdentity guild)
        => new(client, guild, model);

    public ValueTask UpdateAsync(IBanModel model, CancellationToken token = default)
    {
        Model = model;
        return ValueTask.CompletedTask;
    }

    public IBanModel GetModel() => Model;

    public string? Reason => Model.Reason;

}
