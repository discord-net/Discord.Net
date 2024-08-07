using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ChannelPinsUpdated : IGatewayPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("last_pin_timestamp")]
    public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }
}
