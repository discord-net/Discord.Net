using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayGuildEmoteActor :
    GatewayCachedActor<ulong, GatewayGuildEmote, GuildEmoteIdentity, ICustomEmoteModel>,
    IGuildEmoteActor
{
    [SourceOfTruth, StoreRoot] public GatewayGuildActor Guild { get; }

    internal override GuildEmoteIdentity Identity { get; }

    [TypeFactory]
    public GatewayGuildEmoteActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        GuildEmoteIdentity emote
    ) : base(client, emote)
    {
        Identity = emote | this;
        Guild = client.Guilds >> guild;
    }

    [SourceOfTruth]
    internal GatewayGuildEmote CreateEntity(ICustomEmoteModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayGuildEmote :
    GatewayCacheableEntity<GatewayGuildEmote, ulong, ICustomEmoteModel>,
    IGuildEmote
{
    public string Name => Model.Name;

    public bool IsManaged => Model.IsManaged;

    public bool RequireColons => Model.RequireColons;

    public bool IsAnimated => Model.IsAnimated;

    public bool IsAvailable => Model.IsAvailable;

    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    [SourceOfTruth] public GatewayUserActor? Creator { get; private set; }

    [ProxyInterface] internal GatewayGuildEmoteActor Actor { get; }

    internal ICustomEmoteModel Model { get; private set; }

    public GatewayGuildEmote(
        DiscordGatewayClient client,
        GuildIdentity guild,
        ICustomEmoteModel model,
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
        IGatewayConstructionContext context,
        ICustomEmoteModel model
    ) => new(
        client,
        context.Path.RequireIdentity(T<GuildIdentity>()),
        model,
        context.TryGetActor<GatewayGuildEmoteActor>()
    );

    public override ValueTask UpdateAsync(
        ICustomEmoteModel model,
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

    public override ICustomEmoteModel GetModel() => Model;
}
