using Discord.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Sticker;

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
        public SocketGuildUser Author
            => AuthorId.HasValue ? Guild.GetUser(AuthorId.Value) : null;

        /// <summary>
        ///     Gets the guild the sticker was created in.
        /// </summary>
        public SocketGuild Guild { get; }

        /// <inheritdoc/>
        public ulong? AuthorId { get; set; }

        internal SocketCustomSticker(DiscordSocketClient client, ulong id, SocketGuild guild, ulong? authorId = null)
            : base(client, id)
        {
            Guild = guild;
            AuthorId = authorId;
        }

        internal static SocketCustomSticker Create(DiscordSocketClient client, Model model, SocketGuild guild, ulong? authorId = null)
        {
            var entity = new SocketCustomSticker(client, model.Id, guild, authorId);
            entity.Update(model);
            return entity;
        }

        /// <inheritdoc/>
        public async Task ModifyAsync(Action<StickerProperties> func, RequestOptions options = null)
        {
            if (!Guild.CurrentUser.GuildPermissions.Has(GuildPermission.ManageEmojisAndStickers))
                throw new InvalidOperationException($"Missing permission {nameof(GuildPermission.ManageEmojisAndStickers)}");

            var model = await GuildHelper.ModifyStickerAsync(Discord, Guild.Id, this, func, options);

            Update(model);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(RequestOptions options = null)
        {
            await GuildHelper.DeleteStickerAsync(Discord, Guild.Id, this, options);
            Guild.RemoveSticker(Id);
        }

        internal SocketCustomSticker Clone() => MemberwiseClone() as SocketCustomSticker;

        private new string DebuggerDisplay => Guild == null ? base.DebuggerDisplay : $"{Name} in {Guild.Name} ({Id})";
        #endregion

        #region  ICustomSticker
        ulong? ICustomSticker.AuthorId
            => AuthorId;

        IGuild ICustomSticker.Guild
            => Guild;
        #endregion
    }
}
