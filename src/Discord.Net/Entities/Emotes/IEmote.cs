namespace Discord
{
    /// <summary>
    ///     An emote which may be used as a reaction or embedded in chat.
    ///
    ///     This includes Unicode emoji as well as unattached guild emotes.
    /// </summary>
    public interface IEmote : ITaggable
    {
        /// <summary>
        ///     The display-name of the emote.
        /// </summary>
        /// <remarks>
        ///     For Unicode emoji, this is the raw value of the character, not its
        ///     Unicode display name.
        /// </remarks>
        string Name { get; }
    }
}
