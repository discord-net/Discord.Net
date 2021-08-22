using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Model = Discord.API.Sticker;

namespace Discord.Rest
{
    /// <inheritdoc cref="ISticker"/>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Sticker : RestEntity<ulong>, ISticker
    {
        /// <inheritdoc />
        public ulong PackId { get; internal set; }
        /// <inheritdoc />
        public string Name { get; internal set; }
        /// <inheritdoc />
        public string Description { get; internal set; }
        /// <inheritdoc />
        public IReadOnlyCollection<string> Tags { get; internal set; }
        /// <inheritdoc />
        public string Asset { get; internal set; }
        /// <inheritdoc />
        public string PreviewAsset { get; internal set; }
        /// <inheritdoc />
        public StickerFormatType Format { get; internal set; }

        /// <inheritdoc/>
        public string GetStickerUrl()
            => CDN.GetStickerUrl(this.Id, this.Format);

        internal Sticker(BaseDiscordClient client, ulong id)
            : base(client, id) { }
        internal static Sticker Create(BaseDiscordClient client, Model model)
        {
            var entity = new Sticker(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            PackId = model.PackId;
            Name = model.Name;
            Description = model.Desription;
            Tags = model.Tags.IsSpecified ? model.Tags.Value.Split(',').Select(x => x.Trim()).ToArray() : new string[0];
            Asset = model.Asset;
            PreviewAsset = model.PreviewAsset;
            Format = model.FormatType;
        }

        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
