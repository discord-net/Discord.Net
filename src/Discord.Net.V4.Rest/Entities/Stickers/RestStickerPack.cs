using Discord.Models;

namespace Discord.Rest;

using StickersActor = RestManagedIndexableLink<RestStickerActor, ulong, RestSticker, ISticker, IStickerModel>;

[ExtendInterfaceDefaults]
public sealed partial class RestStickerPackActor :
    RestActor<ulong, RestStickerPack, StickerPackIdentity>,
    IStickerPackActor
{
    internal override StickerPackIdentity Identity { get; }

    [TypeFactory]
    public RestStickerPackActor(
        DiscordRestClient client,
        StickerPackIdentity pack
    ) : base(client, pack)
    {
        Identity = pack | this;
    }

    [SourceOfTruth]
    internal RestStickerPack CreateEntity(IStickerPackModel model)
        => RestStickerPack.Construct(Client, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestStickerPack :
    RestEntity<ulong>,
    IStickerPack,
    IConstructable<RestStickerPack, IStickerPackModel, DiscordRestClient>
{
    public string Name => Model.Name;

    [SourceOfTruth]
    public StickersActor Stickers { get; }

    public ulong SkuId => Model.SkuId;

    [SourceOfTruth] public RestStickerActor? CoverSticker { get; }

    public string Description => Model.Description;

    public ulong? BannerAssetId => Model.BannerAssetId;

    [ProxyInterface] internal RestStickerPackActor Actor { get; }

    internal IStickerPackModel Model { get; }

    public RestStickerPack(
        DiscordRestClient client,
        IStickerPackModel model,
        RestStickerPackActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, StickerPackIdentity.Of(this));

        if (model is not IModelSourceOfMultiple<IStickerModel> stickersSource)
            throw new ArgumentException($"Expected {model.GetType()} to provide sticker models");

        Stickers = RestManagedIndexableActor.Create(
            Template.Of<RestStickerActor>(),
            client,
            stickersSource.GetModels(),
            RestSticker.Construct,
            Client.Stickers
        );

        if(model.CoverStickerId.HasValue)
            CoverSticker = client.Stickers[model.CoverStickerId.Value];
    }

    public IStickerPackModel GetModel() => Model;

    public static RestStickerPack Construct(DiscordRestClient client, IStickerPackModel model)
        => new(client, model);
}
