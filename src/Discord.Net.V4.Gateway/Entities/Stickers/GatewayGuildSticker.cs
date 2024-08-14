using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest.Extensions;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayGuildStickerActor :
    GatewayStickerActor,
    IGuildStickerActor,
    IGatewayCachedActor<ulong, GatewayGuildSticker, GuildStickerIdentity, IGuildStickerModel>
{
    [SourceOfTruth] internal override GuildStickerIdentity Identity { get; }

    [SourceOfTruth, StoreRoot] public GatewayGuildActor Guild { get; }

    [TypeFactory]
    public GatewayGuildStickerActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        GuildStickerIdentity sticker
    ) : base(client, sticker)
    {
        Identity = sticker | this;
        Guild = client.Guilds >> guild;
    }

    [SourceOfTruth]
    internal GatewayGuildSticker CreateEntity(IGuildStickerModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayGuildSticker :
    GatewaySticker,
    IGuildSticker,
    ICacheableEntity<GatewayGuildSticker, ulong, IGuildStickerModel>
{
    public bool? IsAvailable => Model.Available;

    [SourceOfTruth] public GatewayMemberActor? Author { get; private set; }

    [ProxyInterface] internal override GatewayGuildStickerActor Actor { get; }

    internal override IGuildStickerModel Model => _model;

    private IGuildStickerModel _model;

    public GatewayGuildSticker(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildStickerModel model,
        GatewayGuildStickerActor? actor = null
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor ?? new GatewayGuildStickerActor(client, guild, GuildStickerIdentity.Of(this));
    }

    public static GatewayGuildSticker Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IGuildStickerModel model
    ) => new(
        client,
        context.Path.RequireIdentity(Template.Of<GuildIdentity>()),
        model,
        context.TryGetActor<GatewayGuildStickerActor>()
    );

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildStickerModel model, bool updateCache = true, CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Author = Author.UpdateFrom(
            model.AuthorId,
            GatewayMemberActor.Factory,
            Client,
            Guild.Identity
        );

        _model = model;

        return base.UpdateAsync(model, false, token);
    }

    public override IGuildStickerModel GetModel() => Model;
}
