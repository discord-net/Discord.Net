using System.Text.Json.Serialization;

namespace Discord.API;

internal class MessageReference
{
    [JsonPropertyName("message_id")]
    public Optional<ulong> MessageId { get; set; }

    [JsonPropertyName("channel_id")]
    public Optional<ulong> ChannelId { get; set; } // Optional when sending, always present when receiving

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("fail_if_not_exists")]
    public Optional<bool> FailIfNotExists { get; set; }
}
