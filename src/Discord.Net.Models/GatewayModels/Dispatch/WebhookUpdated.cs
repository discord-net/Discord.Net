using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class WebhookUpdated : IWebhookUpdatedPayloadData
{
    [JsonPropertyName("guild_Id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }
}
