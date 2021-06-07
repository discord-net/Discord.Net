namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the visibility type for a <see cref="Connection"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/user#connection-object-visibility-types"/>
    /// </remarks>
    public enum VisibilityType
    {
        /// <summary>
        /// Invisible to everyone except the user themselves.
        /// </summary>
        None = 0,

        /// <summary>
        /// Visible to everyone.
        /// </summary>
        Everyone = 1,
    }
}
