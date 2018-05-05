using System;
using System.Diagnostics;
using System.Globalization;

namespace Discord
{
    /// <summary>
    ///     A custom image-based emote.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Emote : IEmote, ISnowflakeEntity
    {
        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public ulong Id { get; }
        /// <summary>
        ///     Gets whether this emote is animated.
        /// </summary>
        public bool Animated { get; }
        /// <summary>
        ///     Gets the date when this emote is created.
        /// </summary>
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <summary>
        ///     Gets the image URL of this emote.
        /// </summary>
        public string Url => CDN.GetEmojiUrl(Id, Animated);

        internal Emote(ulong id, string name, bool animated)
        {
            Id = id;
            Name = name;
            Animated = animated;
        }

        /// <summary>
        ///     Determines whether the specified emote is equal to the current emote.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other == this) return true;

            var otherEmote = other as Emote;
            if (otherEmote == null) return false;

            return string.Equals(Name, otherEmote.Name) && Id == otherEmote.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode() * 397) ^ Id.GetHashCode();
            }
        }

        /// <summary> Parses an <see cref="Emote"/> from its raw format. </summary>
        /// <param name="text">The raw encoding of an emote; for example, <:dab:277855270321782784&gt;.</param>
        /// <returns>An emote.</returns>
        /// <exception cref="ArgumentException">Invalid emote format.</exception>
        public static Emote Parse(string text)
        {
            if (TryParse(text, out Emote result))
                return result;
            throw new ArgumentException("Invalid emote format.", nameof(text));
        }

        /// <summary> Tries to parse an <see cref="Emote"/> from its raw format. </summary>
        /// <param name="text">The raw encoding of an emote; for example, <:dab:277855270321782784&gt;.</param>
        /// <param name="result">An emote.</param>
        public static bool TryParse(string text, out Emote result)
        {
            result = null;
            if (text.Length >= 4 && text[0] == '<' && (text[1] == ':' || (text[1] == 'a' && text[2] == ':')) && text[text.Length - 1] == '>')
            {
                bool animated = text[1] == 'a';
                int startIndex = animated ? 3 : 2;

                int splitIndex = text.IndexOf(':', startIndex);
                if (splitIndex == -1)
                    return false;

                if (!ulong.TryParse(text.Substring(splitIndex + 1, text.Length - splitIndex - 2), NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
                    return false;

                string name = text.Substring(startIndex, splitIndex - startIndex);
                result = new Emote(id, name, animated);
                return true;
            }
            return false;

        }

        private string DebuggerDisplay => $"{Name} ({Id})";
        /// <summary>
        ///     Returns the raw representation of the emote.
        /// </summary>
        public override string ToString() => $"<{(Animated ? "a" : "")}:{Name}:{Id}>";
    }
}
