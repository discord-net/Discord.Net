namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the type of the <see cref="Overwrite"/>.
    /// </summary>
    public enum OverwriteType
    {
        /// <summary>
        /// The type of the <see cref="Overwrite"/> is a <see cref="Models.Role"/>.
        /// </summary>
        Role = 0,

        /// <summary>
        /// The type of the <see cref="Overwrite"/> is a <see cref="Models.GuildMember"/>.
        /// </summary>
        GuildMember = 1,
    }
}
