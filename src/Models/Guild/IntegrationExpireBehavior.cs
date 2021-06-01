using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the integration expire behavior.
    /// </summary>
    public enum IntegrationExpireBehavior
    {
        /// <summary>
        ///     It will remove the role.
        /// </summary>
        RemoveRole = 0,

        /// <summary>
        ///     It will kick the member.
        /// </summary>
        Kick = 1,
    }
}
