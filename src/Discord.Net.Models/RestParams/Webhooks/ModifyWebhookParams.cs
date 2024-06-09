using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyWebhookParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("avatar")]
    public Optional<string?> Avatar { get; set; }

    [JsonPropertyName("channel_id")]
    public Optional<ulong> ChannelId { get; set; }
}
