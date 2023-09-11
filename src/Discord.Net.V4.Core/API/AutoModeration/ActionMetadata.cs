using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ActionMetadata
{
    [JsonPropertyName("channel_id")]
    public Optional<ulong> ChannelId { get; set; }

    [JsonPropertyName("duration_seconds")]
    public Optional<int> DurationSeconds { get; set; }

    [JsonPropertyName("custom_message")]
    public Optional<string> CustomMessage { get; set; }
}
