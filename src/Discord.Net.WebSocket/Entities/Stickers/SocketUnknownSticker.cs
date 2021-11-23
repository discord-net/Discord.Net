using System.Collections.Generic;
using System.Diagnostics;
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
        public override IReadOnlyCollection<string> Tags
            => null;

        /// <inheritdoc/>
        public override string Description
            => null;

        /// <inheritdoc/>
        public override ulong PackId
            => 0;
        /// <inheritdoc/>
        public override bool? IsAvailable
            => null;

        /// <inheritdoc/>
        public override int? SortOrder
            => null;

        /// <inheritdoc/>
        public new StickerType? Type
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
            Name = model.Name;
            Format = model.FormatType;
        }

        /// <summary>
        ///     Attempts to try to find the sticker.
        /// </summary>
        /// <returns>
        ///     The sticker representing this unknown stickers Id, if none is found then <see langword="null"/>.
        /// </returns>
        public Task<SocketSticker> ResolveAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
            => Discord.GetStickerAsync(Id, mode, options);

        private new string DebuggerDisplay => $"{Name} ({Id})";
    }
}
