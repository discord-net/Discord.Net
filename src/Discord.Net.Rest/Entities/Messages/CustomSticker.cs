using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Sticker;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based custom sticker within a guild.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class CustomSticker : Sticker, ICustomSticker
    {
        /// <summary>
        ///     Gets the users id who uploaded the sticker.
        /// </summary>
        /// <remarks>
        ///     In order to get the author id, the bot needs the MANAGE_EMOJIS_AND_STICKERS permission.
        /// </remarks>
        public ulong? AuthorId { get; private set; }

        /// <summary>
        ///     Gets the guild that this custom sticker is in.
        /// </summary>
        /// <remarks>
        ///     <b>Note</b>: This property can be <see langword="null"/> if the sticker wasn't fetched from a guild.
        /// </remarks>
        public RestGuild Guild { get; private set; }

        private ulong GuildId { get; set; }

        internal CustomSticker(BaseDiscordClient client, ulong id, RestGuild guild, ulong? authorId = null)
            : base(client, id)
        {
            AuthorId = authorId;
            Guild = guild;
            GuildId = guild.Id;
        }
        internal CustomSticker(BaseDiscordClient client, ulong id, ulong guildId, ulong? authorId = null)
            : base(client, id)
        {
            AuthorId = authorId;
            GuildId = guildId;
        }

        internal static CustomSticker Create(BaseDiscordClient client, Model model, RestGuild guild, ulong? authorId = null)
        {
            var entity = new CustomSticker(client, model.Id, guild, authorId);
            entity.Update(model);
            return entity;
        }

        internal static CustomSticker Create(BaseDiscordClient client, Model model, ulong guildId, ulong? authorId = null)
        {
            var entity = new CustomSticker(client, model.Id, guildId, authorId);
            entity.Update(model);
            return entity;
        }

        /// <inheritdoc/>
        public Task DeleteAsync(RequestOptions options = null)
            => GuildHelper.DeleteStickerAsync(Discord, GuildId, this, options);

        /// <inheritdoc/>
        public async Task ModifyAsync(Action<StickerProperties> func, RequestOptions options = null)
        {
            var model = await GuildHelper.ModifyStickerAsync(Discord, GuildId, this, func, options);
            Update(model);
        }

        private string DebuggerDisplay => Guild != null ? $"{Name} in {Guild.Name} ({Id})" : $"{Name} ({Id})";

        IGuild ICustomSticker.Guild => Guild;
    }
}
