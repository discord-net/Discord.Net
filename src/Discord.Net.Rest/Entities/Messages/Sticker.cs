using System.Collections.Generic;
using System.Diagnostics;
using Model = Discord.API.Sticker;

namespace Discord
{
    /// <inheritdoc cref="ISticker"/>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Sticker : ISticker
    {
        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public ulong PackId { get; }
        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public string Description { get; }
        /// <inheritdoc />
        public IReadOnlyCollection<string> Tags { get; }
        /// <inheritdoc />
        public string Asset { get; }
        /// <inheritdoc />
        public string PreviewAsset { get; }
        /// <inheritdoc />
        public StickerFormatType FormatType { get; }

        internal Sticker(ulong id, ulong packId, string name, string description, string[] tags, string asset, string previewAsset, StickerFormatType formatType)
        {
            Id = id;
            PackId = packId;
            Name = name;
            Description = description;
            Tags = tags.ToReadOnlyCollection();
            Asset = asset;
            PreviewAsset = previewAsset;
            FormatType = formatType;
        }
        internal static Sticker Create(Model model)
        {
            return new Sticker(model.Id, model.PackId, model.Name, model.Desription,
                model.Tags.IsSpecified ? model.Tags.Value.Split(',') : new string[0],
                model.Asset, model.PreviewAsset, model.FormatType);
        }

        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
