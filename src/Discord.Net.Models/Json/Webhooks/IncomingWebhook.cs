using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 1)]
public sealed class IncomingWebhook : Webhook, IIncomingWebhookModel
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("url")]
    public required string Url { get; set; }
    
    [JsonPropertyName("token")]
    public required string Token { get; set; }
}