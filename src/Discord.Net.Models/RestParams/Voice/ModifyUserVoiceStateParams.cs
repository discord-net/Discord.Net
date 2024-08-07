using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class ModifyUserVoiceStateParams
{
    [JsonPropertyName("channel_id")]
    public Optional<ulong> ChannelId { get; set; }

    [JsonPropertyName("suppress")]
    public Optional<bool> Suppress { get; set; }
}
