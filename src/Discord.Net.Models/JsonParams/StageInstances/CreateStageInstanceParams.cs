using System.Text.Json.Serialization;

namespace Discord.Models;

public sealed class CreateStageInstanceParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("topic")]
    public required string Topic { get; set; }

    [JsonPropertyName("privacy_level")]
    public Optional<int> PrivacyLevel { get; set; }

    [JsonPropertyName("send_start_notification")]
    public Optional<bool> SendStartNotification { get; set; }

    [JsonPropertyName("guild_scheduled_event_id")]
    public Optional<ulong> GuildScheduledEventId { get; set; }
}
