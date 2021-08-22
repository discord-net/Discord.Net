using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a discord sticker pack.
    /// </summary>
    /// <typeparam name="TSticker">The type of the stickers within the collection</typeparam>
    public class StickerPack<TSticker> where TSticker : ISticker
    {
        /// <summary>
        ///     Gets the id of the sticker pack.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        ///     Gets a collection of the stickers in the pack.
        /// </summary>
        public IReadOnlyCollection<TSticker> Stickers { get; }

        /// <summary>
        ///     Gets the name of the sticker pack.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the id of the pack's SKU.
        /// </summary>
        public ulong SkuId { get; }

        /// <summary>
        ///     Gets the id of a sticker in the pack which is shown as the pack's icon.
        /// </summary>
        public ulong? CoverStickerId { get; }

        /// <summary>
        ///     Gets the description of the sticker pack.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Gets the id of the sticker pack's banner image
        /// </summary>
        public ulong BannerAssetId { get; }

        internal StickerPack(string name, ulong id, ulong skuid, ulong? coverStickerId, string description, ulong bannerAssetId, IEnumerable<TSticker> stickers)
        {
            this.Name = name;
            this.Id = id;
            this.SkuId = skuid;
            this.CoverStickerId = coverStickerId;
            this.Description = description;
            this.BannerAssetId = bannerAssetId;

            this.Stickers = stickers.ToImmutableArray();
        }
    }
}
