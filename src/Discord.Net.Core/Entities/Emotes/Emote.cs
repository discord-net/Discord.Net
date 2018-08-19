using System;
using System.Globalization;

namespace Discord
{
    /// <summary>
    ///     A custom image-based emote
    /// </summary>
    public class Emote : IEmote, ISnowflakeEntity
    {
        internal Emote(ulong id, string name, bool animated)
        {
            Id = id;
            Name = name;
            Animated = animated;
        }

        /// <summary>
        ///     Is this emote animated?
        /// </summary>
        public bool Animated { get; }

        public string Url => CDN.GetEmojiUrl(Id, Animated);

        private string DebuggerDisplay => $"{Name} ({Id})";

        /// <summary>
        ///     The display name (tooltip) of this emote
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The ID of this emote
        /// </summary>
        public ulong Id { get; }

        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other == this) return true;

            var otherEmote = other as Emote;
            if (otherEmote == null) return false;

            return string.Equals(Name, otherEmote.Name) && Id == otherEmote.Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode() * 397) ^ Id.GetHashCode();
            }
        }

        /// <summary>
        ///     Parse an Emote from its raw format
        /// </summary>
        /// <param name="text">The raw encoding of an emote; for example, &lt;:dab:277855270321782784&gt;</param>
        /// <returns>An emote</returns>
        public static Emote Parse(string text)
        {
            if (TryParse(text, out var result))
                return result;
            throw new ArgumentException("Invalid emote format", nameof(text));
        }

        public static bool TryParse(string text, out Emote result)
        {
            result = null;
            if (text.Length < 4 || text[0] != '<' || text[1] != ':' && (text[1] != 'a' || text[2] != ':') ||
                text[text.Length - 1] != '>') return false;
            var animated = text[1] == 'a';
            var startIndex = animated ? 3 : 2;

            var splitIndex = text.IndexOf(':', startIndex);
            if (splitIndex == -1)
                return false;

            if (!ulong.TryParse(text.Substring(splitIndex + 1, text.Length - splitIndex - 2), NumberStyles.None,
                CultureInfo.InvariantCulture, out var id))
                return false;

            var name = text.Substring(startIndex, splitIndex - startIndex);
            result = new Emote(id, name, animated);
            return true;
        }

        public override string ToString() => $"<{(Animated ? "a" : "")}:{Name}:{Id}>";
    }
}
