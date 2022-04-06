using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     A struct representing a forum channel tag.
    /// </summary>
    public struct ForumTag
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
        public IEmote Emoji { get; }

        internal ForumTag(ulong id, string name, ulong? emojiId, string emojiName)
        {
            if (emojiId.HasValue && emojiId.Value != 0)
                Emoji = new Emote(emojiId.Value, emojiName, false);
            else if (emojiName != null)
                Emoji = new Emoji(name);
            else
                Emoji = null;

            Id = id;
            Name = name;
        }
    }
}
