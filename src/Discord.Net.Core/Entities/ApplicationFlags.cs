using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents public flags for an application.
    /// </summary>
    public enum ApplicationFlags
    {
        GatewayPresence = 1 << 12,
        GatewayPresenceLimited = 1 << 13,
        GatewayGuildMembers = 1 << 14,
        GatewayGuildMembersLimited = 1 << 15,
        VerificationPendingGuildLimit = 1 << 16,
        Embedded = 1 << 17,
        GatewayMessageContent = 1 << 18,
        GatewayMessageContentLimited =  1 << 19
    }
}
