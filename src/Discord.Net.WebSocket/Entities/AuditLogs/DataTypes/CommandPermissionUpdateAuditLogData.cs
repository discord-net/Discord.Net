using System.Collections.Generic;
using System.Collections.Immutable;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an application command permission update.
/// </summary>
public class CommandPermissionUpdateAuditLogData : ISocketAuditLogData
{
    internal CommandPermissionUpdateAuditLogData(IReadOnlyCollection<ApplicationCommandPermission> before, IReadOnlyCollection<ApplicationCommandPermission> after,
        ulong commandId)
    {
        Before = before;
        After = after;
        Commandid = commandId;
    }

    internal static CommandPermissionUpdateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
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

        return new(before.ToImmutableArray(), after.ToImmutableArray(), entry.TargetId!.Value);
    }

    /// <summary>
    ///     Gets the id of the application command which permissions were updated.
    /// </summary>
    public ulong Commandid { get; }
    
    /// <summary>
    ///     Gets values of the permissions before the change if available.
    /// </summary>
    public IReadOnlyCollection<ApplicationCommandPermission> Before { get; }

    /// <summary>
    ///     Gets values of the permissions after the change if available.
    /// </summary>
    public IReadOnlyCollection<ApplicationCommandPermission> After { get; }
}
