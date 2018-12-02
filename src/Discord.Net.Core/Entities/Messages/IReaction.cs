namespace Discord
{
    /// <summary>
    ///     Represents a generic reaction object.
    /// </summary>
    public interface IReaction
    {
        /// <summary>
        ///     The <see cref="IEmote" /> used in the reaction.
        /// </summary>
        IEmote Emote { get; }
    }
}
