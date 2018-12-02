namespace Discord
{
    /// <summary>
    ///     A Unicode emoji.
    /// </summary>
    public class Emoji : IEmote
    {
        // TODO: need to constrain this to Unicode-only emojis somehow

        /// <inheritdoc />
        public string Name { get; }
        /// <summary>
        ///     Gets the Unicode representation of this emote.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="Emoji.Name"/>.
        /// </returns>
        public override string ToString() => Name;

        /// <summary>
        ///     Initializes a new <see cref="Emoji"/> class with the provided Unicode.
        /// </summary>
        /// <param name="unicode">The pure UTF-8 encoding of an emoji.</param>
        public Emoji(string unicode)
        {
            Name = unicode;
        }

        /// <summary>
        ///     Determines whether the specified emoji is equal to the current one.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
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
