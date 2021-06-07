namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the mfa/2fa level for a <see cref="Guild"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-object-mfa-level"/>
    /// </remarks>
    public enum MfaLevel
    {
        /// <summary>
        /// <see cref="Guild"/> has no MFA/2FA requirement for moderation actions.
        /// </summary>
        None = 0,

        /// <summary>
        /// <see cref="Guild"/> has a 2FA requirement for moderation actions.
        /// </summary>
        Elevated = 1,
    }
}
