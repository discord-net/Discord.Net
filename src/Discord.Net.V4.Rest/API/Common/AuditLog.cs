using System.Text.Json.Serialization;

namespace Discord.API;

internal class AuditLog
{
    [JsonPropertyName("application_commands")]
    public ApplicationCommand[] Commands { get; set; }

    [JsonPropertyName("audit_log_entries")]
    public AuditLogEntry[] Entries { get; set; }

    [JsonPropertyName("auto_moderation_rules")]
    public AutoModerationRule[] AutoModerationRules { get; set;}

    [JsonPropertyName("guild_scheduled_events")]
    public GuildScheduledEvent[] GuildScheduledEvents { get; set; }

    [JsonPropertyName("integrations")]
    public Integration[] Integrations { get; set; }

    [JsonPropertyName("threads")]
    public Channel[] Threads { get; set; }

    [JsonPropertyName("users")]
    public User[] Users { get; set; }

    [JsonPropertyName("webhooks")]
    public Webhook[] Webhooks { get; set; }
}
