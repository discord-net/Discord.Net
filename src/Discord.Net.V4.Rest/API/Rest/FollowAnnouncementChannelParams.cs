using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class FollowAnnouncementChannelParams
{
    [JsonPropertyName("webhook_channel_id")]
    public ulong WebhookChannelId { get; set; }
}
