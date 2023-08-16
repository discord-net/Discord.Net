using Discord.Gateway.Cache;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    /// <summary>
    ///     Represents an unknown sticker received over the gateway.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketUnknownSticker : SocketSticker
    {
        /// <inheritdoc/>
        public override IReadOnlyCollection<string> Tags
            => ImmutableArray<string>.Empty;

        /// <inheritdoc/>
        public override string? Description
            => null;

        /// <inheritdoc/>
        public override ulong? PackId
            => null;

        /// <inheritdoc/>
        public override bool? IsAvailable
            => null;

        /// <inheritdoc/>
        public override int? SortOrder
            => null;

        /// <inheritdoc/>
        public new StickerType? Type
            => null;

        internal SocketUnknownSticker(DiscordGatewayClient client, ulong id, IStickerModel model)
            : base(client, id, model) { }

        /// <summary>
        ///     Attempts to try to find the sticker.
        /// </summary>
        /// <returns>
        ///     The sticker representing this unknown stickers Id, if none is found then <see langword="null" />.
        /// </returns>
        public Task<SocketSticker?> ResolveAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
            => throw new NotImplementedException();

        private new string DebuggerDisplay => $"{Name} ({Id})";
    }
}
