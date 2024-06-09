using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class FollowedChannel
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("webhook_id")]
    public ulong WebhookId { get; set; }
}
