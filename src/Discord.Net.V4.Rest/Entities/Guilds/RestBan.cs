using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestBanActor :
    RestActor<ulong, RestBan, BanIdentity>,
    IBanActor
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public RestUserActor User { get; }

    internal override BanIdentity Identity { get; }

    public RestBanActor(
        DiscordRestClient client,
        GuildIdentity guild,
        BanIdentity ban,
        UserIdentity? user = null
    ) : base(client, ban)
    {
        Identity = ban | this;

        Guild = guild.Actor ?? new(client, guild);
        User = user?.Actor ?? new(client, user ?? UserIdentity.Of(ban.Id));
    }

    [SourceOfTruth]
    internal RestBan CreateEntity(IBanModel model)
        => RestBan.Construct(Client, Guild.Identity, model);
}

public sealed partial class RestBan :
    RestEntity<ulong>,
    IBan,
    IRestConstructable<RestBan, RestBanActor, IBanModel>
{
    public string? Reason => Model.Reason;

    [ProxyInterface(
        typeof(IBanActor),
        typeof(IUserRelationship),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IBan, IBanModel>)
    )]
    internal RestBanActor Actor { get; }

    internal IBanModel Model { get; private set; }

    public RestBan(DiscordRestClient client,
        GuildIdentity guild,
        IBanModel model,
        RestBanActor? actor = null) : base(client, model.UserId)
    {
        Actor = actor ?? new(
            client,
            guild,
            BanIdentity.Of(this),
            UserIdentity.FromReferenced<RestUser, DiscordRestClient>(model, model.UserId, client)
        );
        Model = model;
    }

    public static RestBan Construct(DiscordRestClient client, GuildIdentity guild, IBanModel model)
        => new(client, guild, model);

    public ValueTask UpdateAsync(IBanModel model, CancellationToken token = default)
    {
        Model = model;
        return ValueTask.CompletedTask;
    }

    public IBanModel GetModel() => Model;
}
