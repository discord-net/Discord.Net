using System.Collections.Generic;
using System.Collections.Immutable;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an application command permission update.
/// </summary>
public class SocketCommandPermissionUpdateAuditLogData : ISocketAuditLogData
{
    internal SocketCommandPermissionUpdateAuditLogData(IReadOnlyCollection<ApplicationCommandPermission> before, IReadOnlyCollection<ApplicationCommandPermission> after,
        ulong commandId, ulong appId)
    {
        Before = before;
        After = after;
        ApplicationId = appId;
        CommandId = commandId;
    }

    internal static SocketCommandPermissionUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var changes = entry.Changes;

        var before = new List<ApplicationCommandPermission>();
        var after = new List<ApplicationCommandPermission>();

        foreach (var change in changes)
        {
            var oldValue = change.OldValue?.ToObject<API.ApplicationCommandPermissions>();
            var newValue = change.NewValue?.ToObject<API.ApplicationCommandPermissions>();

            if (oldValue is not null)
                before.Add(new ApplicationCommandPermission(oldValue.Id, oldValue.Type, oldValue.Permission));

            if (newValue is not null)
                after.Add(new ApplicationCommandPermission(newValue.Id, newValue.Type, newValue.Permission));
        }

        return new(before.ToImmutableArray(), after.ToImmutableArray(), entry.TargetId!.Value, entry.Options.ApplicationId!.Value);
    }

    /// <summary>
    ///     Gets the ID of the app whose permissions were targeted.
    /// </summary>
    public ulong ApplicationId { get; set; }

    /// <summary>
    ///     Gets the id of the application command which permissions were updated.
    /// </summary>
    public ulong CommandId { get; }
    
    /// <summary>
    ///     Gets values of the permissions before the change if available.
    /// </summary>
    public IReadOnlyCollection<ApplicationCommandPermission> Before { get; }

    /// <summary>
    ///     Gets values of the permissions after the change if available.
    /// </summary>
    public IReadOnlyCollection<ApplicationCommandPermission> After { get; }
}
