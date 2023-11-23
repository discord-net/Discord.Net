using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord;

/// <summary>
///     Represents public flags for an application.
/// </summary>
[Flags]
public enum ApplicationFlags
{
    /// <summary>
    ///     Indicates if an app uses the Auto Moderation API.
    /// </summary>
    UsesAutoModApi = 1 << 6,

    /// <summary>
    ///     Indicates that the app has been verified to use GUILD_PRESENCES intent.
    /// </summary>
    GatewayPresence = 1 << 12,

    /// <summary>
    ///     Indicates that the app has enabled the GUILD_PRESENCES intent on a bot in less than 100 servers.
    /// </summary>
    GatewayPresenceLimited = 1 << 13,

    /// <summary>
    ///     Indicates that the app has been verified to use GUILD_MEMBERS intent.
    /// </summary>
    GatewayGuildMembers = 1 << 14,

    /// <summary>
    ///     Indicates that the app has enabled the GUILD_MEMBERS intent on a bot in less than 100 servers.
    /// </summary>
    GatewayGuildMembersLimited = 1 << 15,

    /// <summary>
    ///     Indicates unusual growth of an app that prevents verification.
    /// </summary>
    VerificationPendingGuildLimit = 1 << 16,

    /// <summary>
    ///     Indicates if an app is embedded within the Discord client.
    /// </summary>
    Embedded = 1 << 17,

    /// <summary>
    ///     Indicates that the app has been verified to use MESSAGE_CONTENT intent.
    /// </summary>
    GatewayMessageContent = 1 << 18,

    /// <summary>
    ///     Indicates that the app has enabled the MESSAGE_CONTENT intent on a bot in less than 100 servers.
    /// </summary>
    GatewayMessageContentLimited = 1 << 19,

    /// <summary>
    /// 	Indicates if an app has registered global application commands.
    /// </summary>
    ApplicationCommandBadge = 1 << 23,

    /// <summary>
    ///     Indicates if an app is considered active.
    /// </summary>
    ActiveApplication = 1 << 24
}

