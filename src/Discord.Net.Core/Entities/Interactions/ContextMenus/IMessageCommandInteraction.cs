namespace Discord
{
    /// <summary>
    ///     Represents a Message Command interaction.
    /// </summary>
    public interface IMessageCommandInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data associated with this interaction.
        /// </summary>
        new IMessageCommandInteractionData Data { get; }
    }
}
