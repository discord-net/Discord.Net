using Discord.API.AuditLogs;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.WebSocket;

/// <summary>
///     Represents information for an integration.
/// </summary>
public class SocketIntegrationInfo
{
    internal SocketIntegrationInfo(IntegrationInfoAuditLogModel model)
    {
        Name = model.Name;
        Type = model.Type;
        EnableEmojis = model.EnableEmojis;
        Enabled = model.Enabled;
        Scopes = model.Scopes?.ToImmutableArray();
        ExpireBehavior = model.ExpireBehavior;
        ExpireGracePeriod = model.ExpireGracePeriod;
        Syncing = model.Syncing;
        RoleId = model.RoleId;
    }

    /// <summary>
    ///     Gets the name of the integration. <see landword="null"/> if the property was not mentioned in this audit log.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets the type of the integration. <see landword="null"/> if the property was not mentioned in this audit log.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    ///     Gets if the integration is enabled. <see landword="null"/> if the property was not mentioned in this audit log.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    ///     Gets if syncing is enabled for this integration. <see landword="null"/> if the property was not mentioned in this audit log.
    /// </summary>
    public bool? Syncing { get; set; }

    /// <summary>
    ///     Gets the id of the role that this integration uses for subscribers. <see landword="null"/> if the property was not mentioned in this audit log.
    /// </summary>
    public ulong? RoleId { get; set; }

    /// <summary>
    ///     Gets whether emoticons should be synced for this integration. <see landword="null"/> if the property was not mentioned in this audit log.
    /// </summary>
    public bool? EnableEmojis { get; set; }

    /// <summary>
    ///     Gets the behavior of expiring subscribers. <see landword="null"/> if the property was not mentioned in this audit log.
    /// </summary>
    public IntegrationExpireBehavior? ExpireBehavior { get; set; }

    /// <summary>
    /// 	Gets the grace period (in days) before expiring subscribers. <see landword="null"/> if the property was not mentioned in this audit log.
    /// </summary>
    public int? ExpireGracePeriod { get; set; }

    /// <summary>
    ///     Gets the scopes the application has been authorized for. <see landword="null"/> if the property was not mentioned in this audit log.
    /// </summary>
    public IReadOnlyCollection<string> Scopes { get; set; }
}
