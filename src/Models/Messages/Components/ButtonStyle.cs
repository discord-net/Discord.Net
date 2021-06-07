namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the style for a <see cref="ButtonComponent"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/interactions/message-components#buttons-button-styles"/>
    /// </remarks>
    public enum ButtonStyle
    {
        /// <summary>
        /// Blurple.
        /// </summary>
        Primary = 1,

        /// <summary>
        /// Grey.
        /// </summary>
        Secondary = 2,

        /// <summary>
        /// Green.
        /// </summary>
        Success = 3,

        /// <summary>
        /// Red.
        /// </summary>
        Danger = 4,

        /// <summary>
        /// Grey, navigates to a URL.
        /// </summary>
        Link = 5,
    }
}
