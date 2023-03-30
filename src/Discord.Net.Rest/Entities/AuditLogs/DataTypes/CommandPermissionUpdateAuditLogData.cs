using Discord.API;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to an application command permission update.
/// </summary>
public class CommandPermissionUpdateAuditLogData : IAuditLogData
{
    internal CommandPermissionUpdateAuditLogData(IReadOnlyCollection<ApplicationCommandPermission> before, IReadOnlyCollection<ApplicationCommandPermission> after,
        IApplicationCommand command, ulong appId)
    {
        Before = before;
        After = after;
        ApplicationCommand = command;
        ApplicationId = appId;
    }

    internal static CommandPermissionUpdateAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
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

        var command = log.Commands.FirstOrDefault(x => x.Id == entry.TargetId);
        var appCommand = RestApplicationCommand.Create(discord, command, command?.GuildId.IsSpecified ?? false ? command.GuildId.Value : null);

        return new(before.ToImmutableArray(), after.ToImmutableArray(), appCommand, entry.Options.ApplicationId!.Value);
    }

    /// <summary>
    ///     Gets the ID of the app whose permissions were targeted.
    /// </summary>
    public ulong ApplicationId { get; set; }

    /// <summary>
    ///     Gets the application command which permissions were updated.
    /// </summary>
    public IApplicationCommand ApplicationCommand { get; }
    
    /// <summary>
    ///     Gets values of the permissions before the change if available.
    /// </summary>
    public IReadOnlyCollection<ApplicationCommandPermission> Before { get; }

    /// <summary>
    ///     Gets values of the permissions after the change if available.
    /// </summary>
    public IReadOnlyCollection<ApplicationCommandPermission> After { get; }
}
