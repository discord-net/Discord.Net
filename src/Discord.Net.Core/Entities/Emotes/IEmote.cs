namespace Discord
{
    /// <summary>
    ///     Represents a general container for any type of emote in a message.
    /// </summary>
    public interface IEmote
    {
        /// <summary>
        ///     Gets the display name or Unicode representation of this emote.
        /// </summary>
        /// <returns>
        ///     A string representing the display name or the Unicode representation (e.g. <c>🤔</c>) of this emote.
        /// </returns>
        string Name { get; }
    }
}
