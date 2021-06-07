namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the verification level for a <see cref="Guild"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-object-verification-level"/>
    /// </remarks>
    public enum VerificationLevel
    {
        /// <summary>
        /// Unrestricted.
        /// </summary>
        None = 0,

        /// <summary>
        /// Must have verified email on account.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Must be registered on Discord for longer than 5 minutes.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Must be a <see cref="GuildMember"/> of the server for longer than 10 minutes.
        /// </summary>
        High = 3,

        /// <summary>
        /// Must have a verified phone number.
        /// </summary>
        VeryHigh = 4,
    }
}
