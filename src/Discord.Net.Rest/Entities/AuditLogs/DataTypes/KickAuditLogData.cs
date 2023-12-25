using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a kick.
/// </summary>
public class KickAuditLogData : IAuditLogData
{
    private KickAuditLogData(RestUser user, string integrationType)
    {
        Target = user;
        IntegrationType = integrationType;
    }

    internal static KickAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
        return new KickAuditLogData((userInfo != null) ? RestUser.Create(discord, userInfo) : null, entry.Options.IntegrationType);
    }

    /// <summary>
    ///     Gets the user that was kicked.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the user is a 'Deleted User#....' because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A user object representing the kicked user.
    /// </returns>
    public IUser Target { get; }
    
    /// <summary>
    ///     Gets the type of integration which performed the action. <see langword="null"/> if the action was performed by a user.
    /// </summary>
    public string IntegrationType { get; }
}
