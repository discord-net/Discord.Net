namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the invite target type for an <see cref="Invite"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/invite#invite-object-invite-target-types"/>
    /// </remarks>
    public enum InviteTargetType
    {
        /// <summary>
        /// The invite target is a stream.
        /// </summary>
        Stream = 1,

        /// <summary>
        /// The invite target is an embedded application.
        /// </summary>
        EmbeddedApplication = 2,
    }
}
