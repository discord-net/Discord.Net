using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.DM)]
public sealed class DMChannelModel : Channel
{
    [JsonPropertyName("last_message_id")]
    public Optional<ulong?> LastMessageId { get; set; }

    [JsonPropertyName("last_pin_timestamp")]
    public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonPropertyName("recipients")]
    public Optional<User[]> Recipients { get; set; }
}
