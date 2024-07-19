using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Stickers;

public sealed partial class RestStickerItem(DiscordRestClient client, IStickerItemModel model) :
    RestEntity<ulong>(client, model.Id),
    IStickerItem,
    IConstructable<RestStickerItem, IStickerItemModel, DiscordRestClient>
{
    internal IStickerItemModel Model { get; } = model;

    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType)Model.FormatType;

    public static RestStickerItem Construct(DiscordRestClient client, IStickerItemModel model)
        => new(client, model);

    public IStickerItemModel GetModel() => Model;

    [SourceOfTruth]
    internal RestSticker CreateEntity(IStickerModel model)
        => RestSticker.Construct(Client, model);
}
