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
    public struct ForumTag : ISnowflakeEntity, IForumTag
    {
        /// <summary>
        ///     Gets the Id of the tag.
        /// </summary>
        public ulong Id { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IEmote? Emoji { get; }

        /// <inheritdoc/>
        public bool IsModerated { get; }

        /// <inheritdoc/>
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

        public override int GetHashCode() => (Id, Name, Emoji, IsModerated).GetHashCode();

        public override bool Equals(object? obj)
            => obj is ForumTag tag && Equals(tag);

        /// <summary>
        /// Gets whether supplied tag is equals to the current one.
        /// </summary>
        public bool Equals(ForumTag tag)
            => Id == tag.Id &&
               Name == tag.Name &&
               (Emoji is Emoji emoji && tag.Emoji is Emoji otherEmoji && emoji.Equals(otherEmoji) ||
                Emoji is Emote emote && tag.Emoji is Emote otherEmote && emote.Equals(otherEmote)) &&
               IsModerated == tag.IsModerated;

        public static bool operator ==(ForumTag? left, ForumTag? right)
            => left?.Equals(right) ?? right is null;

        public static bool operator !=(ForumTag? left, ForumTag? right) => !(left == right);
    }
}
