using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the m f a level.
    /// </summary>
    public enum MfaLevel
    {
        /// <summary>
        ///     Guild has no MFA/2FA requirement for moderation actions.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Guild has a 2FA requirement for moderation actions.
        /// </summary>
        Elevated = 1,
    }
}
