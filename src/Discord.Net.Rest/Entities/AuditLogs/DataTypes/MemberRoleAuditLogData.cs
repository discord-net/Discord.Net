using System.Collections.Generic;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a change in a guild member's roles.
/// </summary>
public class MemberRoleAuditLogData : IAuditLogData
{
    private MemberRoleAuditLogData(IReadOnlyCollection<MemberRoleEditInfo> roles, IUser target, string integrationType)
    {
        Roles = roles;
        Target = target;
        IntegrationType = integrationType;
    }

    internal static MemberRoleAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log = null)
    {
        var changes = entry.Changes;

        var roleInfos = changes.SelectMany(x => x.NewValue.ToObject<API.Role[]>(discord.ApiClient.Serializer),
                (model, role) => new { model.ChangedProperty, Role = role })
            .Select(x => new MemberRoleEditInfo(x.Role.Name, x.Role.Id, x.ChangedProperty == "$add", x.ChangedProperty == "$remove"))
            .ToList();

        var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
        RestUser user = (userInfo != null) ? RestUser.Create(discord, userInfo) : null;

        return new MemberRoleAuditLogData(roleInfos.ToReadOnlyCollection(), user, entry.Options?.IntegrationType);
    }

    /// <summary>
    ///     Gets a collection of role changes that were performed on the member.
    /// </summary>
    /// <returns>
    ///     A read-only collection of <see cref="MemberRoleEditInfo"/>, containing the roles that were changed on
    ///     the member.
    /// </returns>
    public IReadOnlyCollection<MemberRoleEditInfo> Roles { get; }

    /// <summary>
    ///     Gets the user that the roles changes were performed on.
    /// </summary>
    /// <returns>
    ///     A user object representing the user that the role changes were performed on.
    /// </returns>
    public IUser Target { get; }

    /// <summary>
    ///     Gets the type of integration which performed the action. <see langword="null"/> if the action was performed by a user.
    /// </summary>
    public string IntegrationType { get; }
}
