using Discord.Models;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestStickerPackActor :
    RestActor<RestStickerPackActor, ulong, RestStickerPack, IStickerPackModel>,
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
    internal override RestStickerPack CreateEntity(IStickerPackModel model)
        => RestStickerPack.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestStickerPack :
    RestEntity<ulong>,
    IStickerPack,
    IRestConstructable<RestStickerPack, RestStickerPackActor, IStickerPackModel>
{
    public string Name => Model.Name;

    [SourceOfTruth]
    public RestStickerActor.Defined.Indexable Stickers { get; }

    public ulong SkuId => Model.SkuId;

    [SourceOfTruth] public RestStickerActor? CoverSticker { get; private set; }

    public string Description => Model.Description;

    public ulong? BannerAssetId => Model.BannerAssetId;

    [ProxyInterface] internal RestStickerPackActor Actor { get; }

    internal IStickerPackModel Model { get; private set; }

    public RestStickerPack(
        DiscordRestClient client,
        IStickerPackModel model,
        RestStickerPackActor actor
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor;

        Stickers = new(
            client,
            RestActorProvider.GetOrCreate(
                client,
                Template.Of<StickerIdentity>()
            ),
            model.StickerIds.ToList().AsReadOnly()
        );

        if (model is IModelSourceOfMultiple<IStickerModel> stickers)
            Stickers.AddModelSources(Template.Of<StickerIdentity>(), stickers);

        if(model.CoverStickerId.HasValue)
            CoverSticker = client.Stickers[model.CoverStickerId.Value];
    }

    public static RestStickerPack Construct(
        DiscordRestClient client, 
        RestStickerPackActor actor,
        IStickerPackModel model)
        => new(client, model, actor);

    public ValueTask UpdateAsync(IStickerPackModel model, CancellationToken token = default)
    {
        Model = model;
        
        if (model is IModelSourceOfMultiple<IStickerModel> stickers)
            Stickers.AddModelSources(Template.Of<StickerIdentity>(), stickers);

        CoverSticker = CoverSticker.UpdateFrom(
            model.CoverStickerId,
            RestStickerActor.Factory,
            Client
        );
        
        return ValueTask.CompletedTask;
    }
    
    public IStickerPackModel GetModel() => Model;
}
