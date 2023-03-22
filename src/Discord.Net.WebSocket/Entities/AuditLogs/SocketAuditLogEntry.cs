using Discord.Rest;
using System;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.Gateway.AuditLogCreatedEvent;

namespace Discord.WebSocket;

/// <summary>
///     Represents a Socket-based audit log entry.
/// </summary>
public class SocketAuditLogEntry : SocketEntity<ulong>, IAuditLogEntry
{
    private SocketAuditLogEntry(DiscordSocketClient discord, EntryModel model)
        : base(discord, model.Id)
    {
        Action = model.Action;
        Data = SocketAuditLogHelper.CreateData(discord, model);
        Reason = model.Reason;

        var guild = discord.State.GetGuild(model.GuildId);
        User = guild?.GetUser(model.UserId ?? 0);
    }

    internal static SocketAuditLogEntry Create(DiscordSocketClient discord, EntryModel model)
    {
        return new SocketAuditLogEntry(discord, model);
    }

    /// <inheritdoc/>
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc/>
    public ActionType Action { get; }

    /// <inheritdoc cref="IAuditLogEntry.Data"/>
    public ISocketAuditLogData Data { get; }

    /// <inheritdoc cref="IAuditLogEntry.User" />
    public SocketUser User { get; private set; }

    /// <inheritdoc/>
    public string Reason { get; }

    #region IAuditLogEntry

    /// <inheritdoc/>
    IUser IAuditLogEntry.User => User;

    /// <inheritdoc/>
    IAuditLogData IAuditLogEntry.Data => Data;

    #endregion
}
