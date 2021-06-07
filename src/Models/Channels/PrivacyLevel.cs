namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the privacy level for a <see cref="StageInstance"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/stage-instance#stage-instance-object-privacy-level"/>
    /// </remarks>
    public enum PrivacyLevel
    {
        /// <summary>
        /// The <see cref="StageInstance"/> is visible publicly, such as on Stage discovery.
        /// </summary>
        Public = 1,

        /// <summary>
        /// The <see cref="StageInstance"/> is visible to only <see cref="GuildMember"/>s.
        /// </summary>
        GuildOnly = 2,
    }
}
