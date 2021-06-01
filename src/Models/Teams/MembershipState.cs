using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the membership state type.
    /// </summary>
    public enum MembershipState
    {
        /// <summary>
        ///     This member has been invited, but did not accept yet.
        /// </summary>
        Invited = 1,

        /// <summary>
        ///     This member accepted the invite.
        /// </summary>
        Accepted = 2,
    }
}
