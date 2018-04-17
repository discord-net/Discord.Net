namespace Discord
{
    /// <summary>
    ///     A Unicode emoji.
    /// </summary>
    public class Emoji : IEmote
    {
        // TODO: need to constrain this to Unicode-only emojis somehow
    
        /// <summary>
        ///     Gets the Unicode representation of this emote.
        /// </summary>
        public string Name { get; }
        /// <summary>
        ///     Gets the Unicode representation of this emote.
        /// </summary>
        public override string ToString() => Name;

        /// <summary>
        ///     Creates a Unicode emoji.
        /// </summary>
        /// <param name="unicode">The pure UTF-8 encoding of an emoji.</param>
        public Emoji(string unicode)
        {
            Name = unicode;
        }

        /// <summary>
        ///     Determines whether the specified emoji is equal to the current emoji.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other == this) return true;

            var otherEmoji = other as Emoji;
            if (otherEmoji == null) return false;

            return string.Equals(Name, otherEmoji.Name);
        }

        /// <inheritdoc />
        public override int GetHashCode() => Name.GetHashCode();
    }
}
