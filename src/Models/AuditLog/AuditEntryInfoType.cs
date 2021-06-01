namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the type of the overwritten entity for an audit entry info.
    /// </summary>
    public enum AuditEntryInfoType
    {
        /// <summary>
        ///     The type of the overwritten entity is a role.
        /// </summary>
        Role = 0,
        /// <summary>
        ///     The type of the overwritten entity is a member.
        /// </summary>
        Member = 1,
    }
}
