using Discord.Gateway.State;
using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayStickerActor :
    GatewayCachedActor<ulong, GatewaySticker, StickerIdentity, IStickerModel>,
    IStickerActor
{
    internal override StickerIdentity Identity { get; }

    public GatewayStickerActor(
        DiscordGatewayClient client,
        StickerIdentity sticker
    ) : base(client, sticker)
    {
        Identity = sticker | this;
    }

    [SourceOfTruth]
    internal GatewaySticker CreateEntity(IStickerModel model)
        => Client.StateController.CreateLatent(this, model);
}

public partial class GatewaySticker :
    GatewayCacheableEntity<GatewaySticker, ulong, IStickerModel>,
    ISticker
{
    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType)Model.FormatType;

    public ulong? PackId => Model.PackId;

    public string? Description => Model.Description;

    public IReadOnlyCollection<string> Tags { get; private set; }

    public StickerType Type => (StickerType)Model.Type;

    public int? SortOrder => Model.SortValue;

    [ProxyInterface] internal virtual GatewayStickerActor Actor { get; }

    internal virtual IStickerModel Model => _model;

    private IStickerModel _model;

    public GatewaySticker(
        DiscordGatewayClient client,
        IStickerModel model,
        GatewayStickerActor? actor = null
    ) : base(client, model.Id)
    {
        _model = model;
        Actor = actor ?? new(client, StickerIdentity.Of(this));

        Tags = model.Tags.Split(',').ToImmutableList();
    }

    public static GatewaySticker Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IStickerModel model)
    {
        return model switch
        {
            IGuildStickerModel guildStickerModel => GatewayGuildSticker.Construct(client, context, guildStickerModel),
            _ => new GatewaySticker(client, model, context.TryGetActor<GatewayStickerActor>())
        };
    }

    public override ValueTask UpdateAsync(
        IStickerModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        if (_model.Tags != model.Tags)
            Tags = model.Tags.Split(',').ToImmutableList();

        _model = model;

        return ValueTask.CompletedTask;
    }


    public override IStickerModel GetModel() => Model;
}
