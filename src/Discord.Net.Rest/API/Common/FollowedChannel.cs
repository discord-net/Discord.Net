using Newtonsoft.Json;

namespace Discord.API;

internal class FollowedChannel
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("webhook_id")]
    public ulong WebhookId { get; set; }
}
