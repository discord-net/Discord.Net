using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Gateway;

using StickersActor = GatewayDefinedIndexableLink<GatewayStickerActor, ulong, GatewaySticker, ISticker>;

public sealed partial class GatewayStickerPackActor :
    GatewayCachedActor<ulong, GatewayStickerPack, StickerPackIdentity, IStickerPackModel>,
    IStickerPackActor
{
    internal override StickerPackIdentity Identity { get; }

    public GatewayStickerPackActor(
        DiscordGatewayClient client,
        StickerPackIdentity pack
    ) : base(client, pack)
    {
        Identity = pack | this;
    }

    [SourceOfTruth]
    internal GatewayStickerPack CreateEntity(IStickerPackModel model)
        => new(Client, model);
}

public sealed partial class GatewayStickerPack :
    GatewayCacheableEntity<GatewayStickerPack, ulong, IStickerPackModel>,
    IStickerPack
{
    public string Name => Model.Name;

    [SourceOfTruth] public StickersActor Stickers { get; }

    public ulong SkuId => Model.SkuId;

    [SourceOfTruth] public GatewayStickerActor? CoverSticker { get; private set; }

    public string Description => Model.Description;

    public ulong? BannerAssetId => Model.BannerAssetId;

    [ProxyInterface] internal GatewayStickerPackActor Actor { get; }

    internal IStickerPackModel Model { get; private set; }

    public GatewayStickerPack(
        DiscordGatewayClient client,
        IStickerPackModel model,
        GatewayStickerPackActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(client, StickerPackIdentity.Of(this));
        Model = model;

        Stickers = new(model.StickerIds.ToImmutableList(), Client.Stickers);

        if (model.CoverStickerId.HasValue)
            CoverSticker = client.Stickers[model.CoverStickerId.Value];
    }

    public static GatewayStickerPack Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IStickerPackModel model
    ) => new(client, model, context.TryGetActor<GatewayStickerPackActor>());

    public override ValueTask UpdateAsync(
        IStickerPackModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        if(!Model.StickerIds.SequenceEqual(model.StickerIds))
            Stickers.Ids = model.StickerIds.ToImmutableList();

        CoverSticker = CoverSticker.UpdateFrom(
            model.CoverStickerId,
            static (client, sticker) => client.Stickers >> sticker,
            Client
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IStickerPackModel GetModel() => Model;
}
