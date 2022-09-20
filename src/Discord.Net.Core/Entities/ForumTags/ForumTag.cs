using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Discord
{
    /// <summary>
    ///     A struct representing a forum channel tag.
    /// </summary>
    public struct ForumTag : ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the Id of the tag.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        ///     Gets the name of the tag.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the emoji of the tag or <see langword="null"/> if none is set.
        /// </summary>
        /// <remarks>
        ///     If the emoji is <see cref="Emote"/> only the <see cref="Emote.Id"/> will be populated.
        ///     Use <see cref="IGuild.GetEmoteAsync"/> to get the emoji.
        /// </remarks>
        public IEmote? Emoji { get; }

        /// <summary>
        /// Gets whether this tag can only be added to or removed from threads by a member
        /// with the <see cref="GuildPermissions.ManageThreads"/> permission
        /// </summary>
        public bool IsModerated { get; }

        /// <summary>
        /// Gets when the tag was created.
        /// </summary>
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        internal ForumTag(ulong id, string name, ulong? emojiId = null, string? emojiName = null, bool moderated = false)
        {
            if (emojiId.HasValue && emojiId.Value != 0)
                Emoji = new Emote(emojiId.Value, null, false);
            else if (emojiName != null)
                Emoji = new Emoji(emojiName);
            else
                Emoji = null;

            Id = id;
            Name = name;
            IsModerated = moderated;
        }
    }
}
