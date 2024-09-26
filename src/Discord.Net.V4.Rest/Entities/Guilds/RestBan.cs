using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestBanActor :
    RestActor<RestBanActor, ulong, RestBan, IBanModel>,
    IBanActor
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public RestUserActor User { get; }

    internal override BanIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(ban))]
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
    internal override RestBan CreateEntity(IBanModel model)
        => RestBan.Construct(Client, this, model);
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

    internal RestBan(
        DiscordRestClient client,
        IBanModel model,
        RestBanActor actor
    ) : base(client, model.UserId)
    {
        Actor = actor;
        Model = model;
    }

    public static RestBan Construct(DiscordRestClient client, RestBanActor actor, IBanModel model)
        => new(client, model, actor);

    public ValueTask UpdateAsync(IBanModel model, CancellationToken token = default)
    {
        Model = model;
        return ValueTask.CompletedTask;
    }

    public IBanModel GetModel() => Model;
}