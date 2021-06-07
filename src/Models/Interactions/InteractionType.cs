namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the type of interaction.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/interactions/slash-commands#interaction-interactiontype"/>
    /// </remarks>
    public enum InteractionType
    {
        /// <summary>
        /// Received when registering an interaction, replied with a pong.
        /// </summary>
        Ping = 1,

        /// <summary>
        /// This interaction is from a slash command.
        /// </summary>
        ApplicationCommand = 2,

        /// <summary>
        /// This interaction is from a <see cref="Component"/>.
        /// </summary>
        MessageComponent = 3,
    }
}
