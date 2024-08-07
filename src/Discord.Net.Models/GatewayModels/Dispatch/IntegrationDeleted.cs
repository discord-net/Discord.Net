using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class IntegrationDeleted : IIntegrationDeletedPayloadData
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }
}
