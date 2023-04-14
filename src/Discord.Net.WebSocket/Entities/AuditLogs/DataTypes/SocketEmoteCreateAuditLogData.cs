using System.Linq;

using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket;

/// <summary>
///     Contains a piece of audit log data related to an emoji creation.
/// </summary>
public class SocketEmoteCreateAuditLogData : ISocketAuditLogData
{
    private SocketEmoteCreateAuditLogData(ulong id, string name)
    {
        EmoteId = id;
        Name = name;
    }

    internal static SocketEmoteCreateAuditLogData Create(DiscordSocketClient discord, EntryModel entry)
    {
        var change = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");

        var emoteName = change.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
        return new SocketEmoteCreateAuditLogData(entry.TargetId!.Value, emoteName);
    }

    /// <summary>
    ///     Gets the snowflake ID of the created emoji.
    /// </summary>
    /// <returns>
    ///     A <see cref="ulong"/> representing the snowflake identifier for the created emoji.
    /// </returns>
    public ulong EmoteId { get; }

    /// <summary>
    ///     Gets the name of the created emoji.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the created emoji.
    /// </returns>
    public string Name { get; }
}
