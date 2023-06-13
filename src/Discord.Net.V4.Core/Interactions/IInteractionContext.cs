namespace Discord
{
    /// <summary>
    ///     Represents the context of an Interaction.
    /// </summary>
    public interface IInteractionContext
    {
        /// <summary>
        ///     Gets the client that will be used to handle this interaction.
        /// </summary>
        IDiscordClient Client { get; }

        /// <summary>
        ///     Gets the guild the interaction originated from.
        /// </summary>
        /// <remarks>
        ///     Will be <see langword="null"/> if the interaction originated from a DM channel or the interaction was a Context Command interaction.
        /// </remarks>
        IGuild Guild { get; }

        /// <summary>
        ///     Gets the channel the interaction originated from.
        /// </summary>
        IMessageChannel Channel { get; }

        /// <summary>
        ///     Gets the user who invoked the interaction event.
        /// </summary>
        IUser User { get; }

        /// <summary>
        ///     Gets the underlying interaction.
        /// </summary>
        IDiscordInteraction Interaction { get; }
    }
}
