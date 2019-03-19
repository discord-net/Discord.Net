using System;

namespace Discord
{
    /// <summary>
    ///     Methods to create an <see cref="IEmote"/>.
    /// </summary>
    public static class EmoteBuilder
    {
        /// <summary>
        ///     Creates an emote from a raw unicode emoji.
        /// </summary>
        /// <param name="emoji">The unicode character this emoji should be created from.</param>
        public static IEmote FromUnicodeEmoji(string emoji)
            => new Emoji(emoji);

        /// <summary>
        ///     Creates an emote from an escaped tag.
        /// </summary>
        /// <param name="mention">The escaped mention tag for an emote.</param>
        /// <exception cref="ArgumentException">Throws if the passed tag was of an invalid format.</exception>
        public static IEmote FromTag(string tag)
        {
            if (EmoteUtilities.TryParseGuildEmote(tag.AsSpan(), out var result))
            {
                var (id, name) = result;
                return new Emote(id, name);
            }
            throw new ArgumentException("Passed emote tag was of an invalid format", nameof(tag));
        }

        /// <summary>
        ///     Creates an emote from an escaped tag.
        /// </summary>
        /// <param name="tag">The escaped mention tag for an emote.</param>
        /// <returns>Returns true if the emote could be created; returns false if it was of an invalid format.</returns>
        public static bool TryFromTag(string tag, out IEmote result)
        {
            if (EmoteUtilities.TryParseGuildEmote(tag.AsSpan(), out var r))
            {
                var (id, name) = r;
                result = new Emote(id, name);
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        ///     Creates an emote from a raw snowflake and name.
        /// </summary>
        /// <param name="id">The snowflake ID of the guild emote.</param>
        /// <param name="name">The name of the guild emote.</param>
        public static IEmote FromID(ulong id, string name)
            => new Emote(id, name);
    }
}
