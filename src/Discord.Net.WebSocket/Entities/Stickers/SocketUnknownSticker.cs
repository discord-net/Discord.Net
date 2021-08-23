using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.StickerItem;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents an unknown sticker received over the gateway.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketUnknownSticker : SocketSticker
    {
        /// <inheritdoc/>
        public override string Asset
            => null;

        /// <inheritdoc/>
        public override IReadOnlyCollection<string> Tags
            => null;

        /// <inheritdoc/>
        public override string Description
            => null;

        /// <inheritdoc/>
        public override ulong PackId
            => 0;

        /// <inheritdoc/>
        public override string PreviewAsset
            => null;

        internal SocketUnknownSticker(DiscordSocketClient client, ulong id)
            : base(client, id) { }

        internal static SocketUnknownSticker Create(DiscordSocketClient client, Model model)
        {
            var entity = new SocketUnknownSticker(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            this.Name = model.Name;
            this.Format = model.FormatType;
        }

        /// <summary>
        ///     Attempts to try to find the sticker.
        /// </summary>
        /// <returns>
        ///     The sticker representing this unknown stickers Id, if none is found then <see langword="null"/>.
        /// </returns>
        public Task<SocketSticker> ResolveAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
            => Discord.GetStickerAsync(this.Id, mode, options);

        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
