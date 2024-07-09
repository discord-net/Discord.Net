using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Stickers;

public sealed partial class RestStickerItem(DiscordRestClient client, IStickerItemModel model) :
    RestEntity<ulong>(client, model.Id),
    IStickerItem,
    IConstructable<RestStickerItem, IStickerItemModel, DiscordRestClient>
{
    internal IStickerItemModel Model { get; } = model;

    [ProxyInterface(typeof(ILoadableEntity<ISticker>))]
    internal RestLoadable<ulong, RestSticker, ISticker, IStickerModel> Loadable { get; } =
        RestLoadable<ulong, RestSticker, ISticker, IStickerModel>.FromConstructable<RestSticker>(
            client,
            StickerIdentity.Of(model.Id),
            Routes.GetSticker
        );

    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType)Model.FormatType;

    public static RestStickerItem Construct(DiscordRestClient client, IStickerItemModel model)
        => new(client, model);
}
