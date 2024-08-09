using Discord.Models;

namespace Discord.Rest;

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
    public IReadOnlyCollection<RestSticker> Stickers { get; }

    public ulong SkuId => Model.SkuId;

    [SourceOfTruth]
    public RestSticker? CoverSticker { get; }

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

        var stickers = new List<RestSticker>();

        foreach (var stickerModel in model.Stickers)
        {
            var sticker = RestSticker.Construct(client, Actor.Identity, stickerModel);

            if (stickerModel.Id == model.CoverStickerId)
                CoverSticker = sticker;

            stickers.Add(sticker);
        }

        Stickers = stickers.AsReadOnly();
    }

    public IStickerPackModel GetModel() => Model;

    public static RestStickerPack Construct(DiscordRestClient client, IStickerPackModel model)
        => new(client, model);
}
