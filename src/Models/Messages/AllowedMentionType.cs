namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the allowed mention types flags for a <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#allowed-mentions-object-allowed-mention-types"/>
    /// </remarks>
    public enum AllowedMentionType
    {
        /// <summary>
        /// Controls role mentions.
        /// </summary>
        Role,

        /// <summary>
        /// Controls user mentions.
        /// </summary>
        User,

        /// <summary>
        /// Controls @everyone and @here mentions.
        /// </summary>
        Everyone,
    }
}
