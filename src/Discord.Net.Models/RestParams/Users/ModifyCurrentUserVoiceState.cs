using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyCurrentUserVoiceState
{
    [JsonPropertyName("channel_id")]
    public Optional<ulong?> ChannelId { get; set; }

    [JsonPropertyName("suppress")]
    public Optional<bool> Suppress { get; set; }

    [JsonPropertyName("request_to_speak_timestamp")]
    public Optional<DateTimeOffset?> RequestToSpeakTimestamp { get; set; }
}
