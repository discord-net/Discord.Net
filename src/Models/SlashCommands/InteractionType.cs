namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the interaction type.
    /// </summary>
    public enum InteractionType
    {
        /// <summary>
        ///     Received when registering an interaction, replied with a pong.
        /// </summary>
        Ping = 1,

        /// <summary>
        ///     This interaction is from a slash command.
        /// </summary>
        ApplicationCommand = 2,

        /// <summary>
        ///     This interaction is from a component.
        /// </summary>
        MessageComponent = 3,
    }
}
