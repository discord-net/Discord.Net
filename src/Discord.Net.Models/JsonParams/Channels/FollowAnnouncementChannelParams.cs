using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class FollowAnnouncementChannelParams
{
    [JsonPropertyName("webhook_channel_id")]
    public ulong WebhookChannelId { get; set; }
}
