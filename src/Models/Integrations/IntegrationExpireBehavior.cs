namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the integration expire behavior for an <see cref="Integration"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#integration-application-object-integration-application-structure"/>
    /// </remarks>
    public enum IntegrationExpireBehavior
    {
        /// <summary>
        /// It will remove the role.
        /// </summary>
        RemoveRole = 0,

        /// <summary>
        /// It will kick the member.
        /// </summary>
        Kick = 1,
    }
}
