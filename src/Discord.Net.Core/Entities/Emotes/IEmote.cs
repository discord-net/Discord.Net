namespace Discord
{
    /// <summary> Represents a general container for any type of emote in a message. </summary>
    public interface IEmote
    {
        /// <summary> Gets the display name or Unicode representation of this emote. </summary>
        string Name { get; }
    }
}
