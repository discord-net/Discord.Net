namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the type of the overwritten entity for an <see cref="OptionalAuditEntryInfo"/>.
    /// </summary>
    public enum AuditEntryInfoType
    {
        /// <summary>
        /// The type of the overwritten entity is a <see cref="Role"/>.
        /// </summary>
        Role = 0,

        /// <summary>
        /// The type of the overwritten entity is a <see cref="GuildMember"/>.
        /// </summary>
        GuildMember = 1,
    }
}
