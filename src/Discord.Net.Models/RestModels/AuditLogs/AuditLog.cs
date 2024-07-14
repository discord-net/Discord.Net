using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AuditLog :
    IModelSourceOfMultiple<IGuildScheduledEventModel>,
    IModelSourceOfMultiple<IIntegrationModel>,
    IModelSourceOfMultiple<IThreadChannelModel>,
    IModelSourceOfMultiple<IUserModel>,
    IModelSourceOfMultiple<IWebhookModel>
{
    [JsonPropertyName("application_commands")]
    public required ApplicationCommand[] Commands { get; set; }

    [JsonPropertyName("audit_log_entries")]
    public required AuditLogEntry[] Entries { get; set; }

    [JsonPropertyName("auto_moderation_rules")]
    public required AutoModerationRule[] AutoModerationRules { get; set;}

    [JsonPropertyName("guild_scheduled_events")]
    public required GuildScheduledEvent[] GuildScheduledEvents { get; set; }

    [JsonPropertyName("integrations")]
    public required Integration[] Integrations { get; set; }

    [JsonPropertyName("threads")]
    public required ThreadChannelBase[] Threads { get; set; }

    [JsonPropertyName("users")]
    public required User[] Users { get; set; }

    [JsonPropertyName("webhooks")]
    public required Webhook[] Webhooks { get; set; }

    IEnumerable<IGuildScheduledEventModel> IModelSourceOfMultiple<IGuildScheduledEventModel>.GetModels() => GuildScheduledEvents;

    IEnumerable<IIntegrationModel> IModelSourceOfMultiple<IIntegrationModel>.GetModels() => Integrations;

    IEnumerable<IThreadChannelModel> IModelSourceOfMultiple<IThreadChannelModel>.GetModels() => Threads;

    IEnumerable<IUserModel> IModelSourceOfMultiple<IUserModel>.GetModels() => Users;

    IEnumerable<IWebhookModel> IModelSourceOfMultiple<IWebhookModel>.GetModels() => Webhooks;

    public IEnumerable<IEntityModel> GetDefinedModels()
        => [..GuildScheduledEvents, ..Integrations, ..Threads, ..Users, ..Webhooks];
}
