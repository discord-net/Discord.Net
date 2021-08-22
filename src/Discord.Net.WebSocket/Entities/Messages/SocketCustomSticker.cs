using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Sticker;


namespace Discord.WebSocket
{
    public class SocketCustomSticker : SocketSticker, ICustomSticker
    {
        /// <summary>
        ///     Gets the user that uploaded the guild sticker.
        /// </summary>
        /// <remarks>
        ///     <note>
        ///         This may return <see langword="null"/> in the WebSocket implementation due to incomplete user collection in
        ///         large guilds, or the bot doesnt have the MANAGE_EMOJIS_AND_STICKERS permission.
        ///     </note>
        /// </remarks>
        public SocketGuildUser Author
            => this.AuthorId.HasValue ? Guild.GetUser(this.AuthorId.Value) : null;

        /// <summary>
        ///     Gets the guild the sticker lives in.
        /// </summary>
        public SocketGuild Guild { get; }

        /// <inheritdoc/>
        public ulong? AuthorId { get; set; }

        internal SocketCustomSticker(DiscordSocketClient client, ulong id, SocketGuild guild, ulong? authorId = null)
            : base(client, id)
        {
            this.Guild = guild;
            this.AuthorId = authorId;
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
            if(!Guild.CurrentUser.GuildPermissions.Has(GuildPermission.ManageEmojisAndStickers))
                throw new InvalidOperationException($"Missing permission {nameof(GuildPermission.ManageEmojisAndStickers)}");

            var model = await GuildHelper.ModifyStickerAsync(this.Discord, this.Guild, this, func, options);

            this.Update(model);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(RequestOptions options = null)
        {
            await GuildHelper.DeleteStickerAsync(Discord, Guild, this, options);
            Guild.RemoveSticker(this.Id);
        }

        // ICustomSticker
        ulong? ICustomSticker.AuthorId
            => this.AuthorId;

        IGuild ICustomSticker.Guild
            => this.Guild;
    }
}
