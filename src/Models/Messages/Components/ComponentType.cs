namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the type for a <see cref="Component"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/interactions/message-components#component-types"/>
    /// </remarks>
    public enum ComponentType
    {
        /// <summary>
        /// A container for other components.
        /// </summary>
        ActionRow = 1,

        /// <summary>
        /// A clickable button.
        /// </summary>
        Button = 2,
    }
}
