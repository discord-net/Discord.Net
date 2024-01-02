using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class FollowAnnouncementChannelParams
{
    [JsonPropertyName("webhook_channel_id")]
    public ulong WebhookChannelId { get; set; }
}
