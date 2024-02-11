using Discord.Rest;
using System.Collections.Generic;
using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to a change in a guild member's roles.
/// </summary>
public class SocketMemberRoleAuditLogData : ISocketAuditLogData
{
    private SocketMemberRoleAuditLogData(IReadOnlyCollection<SocketMemberRoleEditInfo> roles, Cacheable<SocketUser, RestUser, IUser, ulong> target, string integrationType)
    {
        Roles = roles;
        Target = target;
        IntegrationType = integrationType;
    }

    internal static SocketMemberRoleAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var roleInfos = changes.SelectMany(x => x.NewValue.ToObject<API.Role[]>(discord.ApiClient.Serializer),
                (model, role) => new { model.ChangedProperty, Role = role })
            .Select(x => new SocketMemberRoleEditInfo(x.Role.Name, x.Role.Id, x.ChangedProperty == "$add", x.ChangedProperty == "$remove"))
            .ToList();

        var cachedUser = discord.GetUser(entry.TargetId!.Value);
        var cacheableUser = new Cacheable<SocketUser, RestUser, IUser, ulong>(
            cachedUser,
            entry.TargetId.Value,
            cachedUser is not null,
            async () =>
            {
                var user = await discord.ApiClient.GetUserAsync(entry.TargetId.Value);
                return user is not null ? RestUser.Create(discord, user) : null;
            });

        return new SocketMemberRoleAuditLogData(roleInfos.ToReadOnlyCollection(), cacheableUser, entry.Options?.IntegrationType);
    }

    /// <summary>
    ///     Gets a collection of role changes that were performed on the member.
    /// </summary>
    /// <returns>
    ///     A read-only collection of <see cref="SocketMemberRoleEditInfo"/>, containing the roles that were changed on
    ///     the member.
    /// </returns>
    public IReadOnlyCollection<SocketMemberRoleEditInfo> Roles { get; }

    /// <summary>
    ///     Gets the user that the roles changes were performed on.
    /// </summary>
    /// <returns>
    ///     A cacheable user object representing the user that the role changes were performed on.
    /// </returns>
    public Cacheable<SocketUser, RestUser, IUser, ulong> Target { get; }

    /// <summary>
    ///     Gets the type of integration which performed the action. <see langword="null"/> if the action was performed by a user.
    /// </summary>
    public string IntegrationType { get; }
}
