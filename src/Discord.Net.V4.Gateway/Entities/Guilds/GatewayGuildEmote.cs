using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayGuildEmoteActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    GuildEmoteIdentity emote
) :
    GatewayCachedActor<ulong, GatewayGuildEmote, GuildEmoteIdentity, IGuildEmoteModel>(client, emote),
    IGuildEmoteActor
{
    [SourceOfTruth, StoreRoot] public GatewayGuildActor Guild { get; } = guild.Actor ?? new(client, guild);

    [SourceOfTruth]
    internal GatewayGuildEmote CreateEntity(IGuildEmoteModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayGuildEmote :
    GatewayCacheableEntity<GatewayGuildEmote, ulong, IGuildEmoteModel>,
    IGuildEmote
{
    public string? Name => Model.Name;

    public bool IsManaged => Model.IsManaged;

    public bool RequireColons => Model.RequireColons;

    public bool IsAnimated => Model.IsAnimated;

    public bool IsAvailable => Model.IsAvailable;

    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    [SourceOfTruth]
    public GatewayUserActor? Creator { get; private set; }

    [ProxyInterface] internal GatewayGuildEmoteActor Actor { get; }

    internal IGuildEmoteModel Model { get; private set; }

    public GatewayGuildEmote(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildEmoteModel model,
        GatewayGuildEmoteActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, GuildEmoteIdentity.Of(this));

        Creator = model.UserId.Map(
            static (id, client) => client.Users[id],
            client
        );
    }

    public static GatewayGuildEmote Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext context,
        IGuildEmoteModel model
    ) => new(
        client,
        context.Path.RequireIdentity(T<GuildIdentity>()),
        model,
        context.TryGetActor<GatewayGuildEmoteActor>()
    );

    public override ValueTask UpdateAsync(
        IGuildEmoteModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Creator = Creator.UpdateFrom(
            model.UserId,
            (client, identity) => client.Users[identity.Id],
            Client
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IGuildEmoteModel GetModel() => Model;
}
