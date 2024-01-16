using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyUserVoiceState
{
    [JsonPropertyName("channel_id")]
    public Optional<ulong?> ChannelId { get; set; }

    [JsonPropertyName("suppress")]
    public Optional<bool> Suppress { get; set; }
}
