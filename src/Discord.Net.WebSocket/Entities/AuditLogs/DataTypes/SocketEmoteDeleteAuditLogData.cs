using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an emoji deletion.
/// </summary>
public class SocketEmoteDeleteAuditLogData : ISocketAuditLogData
{
    private SocketEmoteDeleteAuditLogData(ulong id, string name)
    {
        EmoteId = id;
        Name = name;
    }

    internal static SocketEmoteDeleteAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var change = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");

        var emoteName = change.OldValue?.ToObject<string>(discord.ApiClient.Serializer);

        return new SocketEmoteDeleteAuditLogData(entry.TargetId!.Value, emoteName);
    }

    /// <summary>
    ///     Gets the snowflake ID of the deleted emoji.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the deleted emoji.
    /// </returns>
    public ulong EmoteId { get; }

    /// <summary>
    ///     Gets the name of the deleted emoji.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the deleted emoji.
    /// </returns>
    public string Name { get; }
}
