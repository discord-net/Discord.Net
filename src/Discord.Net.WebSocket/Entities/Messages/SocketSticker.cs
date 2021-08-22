using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Sticker;

namespace Discord.WebSocket
{
    public class SocketSticker : SocketEntity<ulong>, ISticker
    {
        /// <inheritdoc/>
        public ulong PackId { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> Tags { get; private set; }

        /// <inheritdoc/>
        public string Asset { get; private set; }

        /// <inheritdoc/>
        public string PreviewAsset { get; private set; }

        /// <inheritdoc/>
        public StickerFormatType FormatType { get; private set; }

        /// <inheritdoc/>
        public string GetStickerUrl()
            => CDN.GetStickerUrl(this.Id, this.FormatType);

        internal SocketSticker(DiscordSocketClient client, ulong id)
            : base(client, id) { }

        internal static SocketSticker Create(DiscordSocketClient client, Model model)
        {
            var entity = new SocketSticker(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            this.Name = model.Name;
            this.Description = model.Desription;
            this.PackId = model.PackId;
            this.Asset = model.Asset;
            this.PreviewAsset = model.PreviewAsset;
            this.FormatType = model.FormatType;

            if (model.Tags.IsSpecified)
            {
                this.Tags = model.Tags.Value.Split(',').Select(x => x.Trim()).ToImmutableArray();
            }
            else
            {
                this.Tags = ImmutableArray<string>.Empty;
            }
        }
    }
}
