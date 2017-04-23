namespace Discord
{
    /// <summary>
    /// A general container for any type of emote in a message.
    /// </summary>
    public interface IEmote
    {
        /// <summary>
        /// The display name or unicode representation of this emote
        /// </summary>
        string Name { get; }
    }
}
