using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public sealed partial class RestStickerItem(
    DiscordRestClient client,
    IStickerItemModel model
) :
    RestEntity<ulong>(client, model.Id),
    IStickerItem,
    IModelConstructable<RestStickerItem, IStickerItemModel, DiscordRestClient>
{
    internal IStickerItemModel Model { get; } = model;

    public string Name => Model.Name;

    public StickerFormatType Format => (StickerFormatType) Model.FormatType;

    public static RestStickerItem Construct(DiscordRestClient client, IStickerItemModel model)
        => new(client, model);

    public IStickerItemModel GetModel() => Model;

    [SourceOfTruth]
    internal RestSticker CreateEntity(IStickerModel model)
        => Client.Stickers[model.Id].CreateEntity(model);
}