namespace Discord
{
    /// <summary>
    /// A unicode emoji
    /// </summary>
    public class Emoji : IEmote
    {
        // TODO: need to constrain this to unicode-only emojis somehow
        /// <summary>
        /// Creates a unicode emoji.
        /// </summary>
        /// <param name="unicode">The pure UTF-8 encoding of an emoji</param>
        public Emoji(string unicode)
        {
            Name = unicode;
        }

        /// <summary>
        /// The unicode representation of this emote.
        /// </summary>
        public string Name { get; }
    }
}
