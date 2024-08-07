using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildScheduledEventUserPayload : IGuildScheduledEventUserPayloadData
{
    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong GuildScheduledEventId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
}
