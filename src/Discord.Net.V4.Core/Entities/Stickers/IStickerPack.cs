using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord;

public interface IStickerPack : ISnowflakeEntity
{
    /// <summary>
    ///     Gets the name of the sticker pack.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets a collection of the stickers in the pack.
    /// </summary>
    IReadOnlyCollection<ISticker> Stickers { get; }

    /// <summary>
    ///     Gets the id of the pack's SKU.
    /// </summary>
    ulong SkuId { get; }

    /// <summary>
    ///     Gets the sticker in the pack which is shown as the pack's icon.
    /// </summary>
    ISticker? CoverSticker { get; }

    /// <summary>
    ///     Gets the description of the sticker pack.
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     Gets the id of the sticker pack's banner image
    /// </summary>
    ulong? BannerAssetId { get; }
}
