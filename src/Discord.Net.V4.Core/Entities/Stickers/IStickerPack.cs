using Discord.Models;
using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.ListStickerPacks))]
public partial interface IStickerPack :
    IStickerPackActor,
    ISnowflakeEntity<IStickerPackModel>
{
    /// <summary>
    ///     Gets the name of the sticker pack.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets a collection of the stickers in the pack.
    /// </summary>
    DefinedIndexableStickerLink Stickers { get; }

    /// <summary>
    ///     Gets the id of the pack's SKU.
    /// </summary>
    ulong SkuId { get; }

    /// <summary>
    ///     Gets the sticker in the pack which is shown as the pack's icon.
    /// </summary>
    IStickerActor? CoverSticker { get; }

    /// <summary>
    ///     Gets the description of the sticker pack.
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     Gets the id of the sticker pack's banner image
    /// </summary>
    ulong? BannerAssetId { get; }
}
