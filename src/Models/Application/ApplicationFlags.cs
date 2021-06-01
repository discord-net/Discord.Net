using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the application flags.
    /// </summary>
    [Flags]
    public enum ApplicationFlags
    {
        /// <summary>
        ///     This application has the gateway presence privileged intent.
        /// </summary>
        GatewayPresence = 1 << 12,

        /// <summary>
        ///     This application has the gateway presence limited.
        /// </summary>
        GatewayPresenceLimited = 1 << 13,

        /// <summary>
        ///     This application has the gateway guild members privileged intent.
        /// </summary>
        GatewayGuildMembers = 1 << 14,

        /// <summary>
        ///     This application has the gateway guid members limited.
        /// </summary>
        GatewayGuildMembersLimited = 1 << 15,

        /// <summary>
        ///     This application has the verification for the increase of the guild limit pending.
        /// </summary>
        VerificationPendingGuildLimit = 1 << 16,

        /// <summary>
        ///     This application is embedded.
        /// </summary>
        Embedded = 1 << 17,
    }
}
