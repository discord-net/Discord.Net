using Discord.Rest;
using Discord.WebSocket.Cache;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a custom sticker within a guild received over the gateway.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketCustomSticker : SocketSticker, ICustomSticker
    {
        #region SocketCustomSticker
        /// <summary>
        ///     Gets the user that uploaded the guild sticker.
        /// </summary>
        /// <remarks>
        ///     <note>
        ///         This may return <see langword="null"/> in the WebSocket implementation due to incomplete user collection in
        ///         large guilds, or the bot doesn't have the MANAGE_EMOJIS_AND_STICKERS permission.
        ///     </note>
        /// </remarks>
        public GuildUserCacheable? Author { get; }

        /// <summary>
        ///     Gets the guild the sticker was created in.
        /// </summary>
        public GuildCacheable Guild { get; }

        /// <inheritdoc/>
        public ulong? AuthorId { get; set; }

        internal SocketCustomSticker(DiscordSocketClient client, IStickerModel model, ulong guildId)
            : base(client, model)
        {
            Guild = new(guildId, Discord, Discord.State.Guilds.SourceSpecific(guildId));
            AuthorId = model.AuthorId;
            Author = model.AuthorId.HasValue
                ? new(model.AuthorId.Value, Discord, Discord.State.Members.SourceSpecific(model.AuthorId.Value))
                : null;
        }

        /// <inheritdoc/>
        public Task ModifyAsync(Action<StickerProperties> func, RequestOptions? options = null)
            => throw new NotImplementedException();

        /// <inheritdoc/>
        public Task DeleteAsync(RequestOptions? options = null)
            => throw new NotImplementedException();

        private new string DebuggerDisplay => Guild == null ? base.DebuggerDisplay : $"{Name} in {Guild.Value?.Name ?? "Unknown Guild"} ({Id})";
        #endregion

        #region  ICustomSticker
        ulong? ICustomSticker.AuthorId
            => AuthorId;

        IGuild ICustomSticker.Guild
            => Guild;
        #endregion
    }
}
