using Discord.Rest;
using Discord.Gateway.Cache;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    /// <summary>
    ///     Represents a custom sticker within a guild received over the gateway.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class GatewayCustomSticker : GatewaySticker, ICustomSticker
    {
        #region SocketCustomSticker
        /// <summary>
        ///     Gets the user that uploaded the guild sticker.
        /// </summary>
        /// <remarks>
        ///     <note>
        ///         This may return <see langword="null" /> in the WebSocket implementation due to incomplete user collection in
        ///         large guilds, or the bot doesn't have the MANAGE_EMOJIS_AND_STICKERS permission.
        ///     </note>
        /// </remarks>
        public GuildUserCacheable? Author { get; }

        /// <summary>
        ///     Gets the guild the sticker was created in.
        /// </summary>
        public GuildCacheable Guild { get; }

        internal GatewayCustomSticker(DiscordGatewayClient client, IStickerModel model, ulong guildId)
            : base(client, model)
        {
            Guild = new(guildId, Discord, Discord.State.Guilds.ProvideSpecific(guildId));
            Author = model.AuthorId.HasValue
                ? new(model.AuthorId.Value, Discord, Discord.State.Members.ProvideSpecific(model.AuthorId.Value))
                : null;
        }

        /// <inheritdoc/>
        public Task ModifyAsync(Action<ModifyStickerProperties> func, RequestOptions? options = null)
            => throw new NotImplementedException();

        /// <inheritdoc/>
        public Task DeleteAsync(RequestOptions? options = null)
            => throw new NotImplementedException();

        private new string DebuggerDisplay => Guild == null ? base.DebuggerDisplay : $"{Name} in {Guild.Value?.Name ?? "Unknown Guild"} ({Id})";
        #endregion

        #region  ICustomSticker

        ILoadableEntity<IGuild, ulong> ICustomSticker.Guild => Guild;

        ILoadableEntity<IGuildMember, ulong>? ICustomSticker.Author => Author;
        #endregion
    }
}
