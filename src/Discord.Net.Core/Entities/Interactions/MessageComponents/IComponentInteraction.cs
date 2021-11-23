namespace Discord
{
    /// <summary>
    ///     Represents an interaction type for Message Components.
    /// </summary>
    public interface IComponentInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data received with this interaction, contains the button that was clicked.
        /// </summary>
        new IComponentInteractionData Data { get; }

        /// <summary>
        ///     Gets the message that contained the trigger for this interaction.
        /// </summary>
        IUserMessage Message { get; }
    }
}
