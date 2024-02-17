using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.API;

[ChannelTypeOf(ChannelType.DM)]
public sealed class DMChannelModel : Channel
{
    [JsonPropertyName("last_message_id")]
    public Optional<ulong?> LastMessageId { get; set; }

    [JsonPropertyName("flags")]
    public Optional<ChannelFlags> Flags { get; set; }

    [JsonPropertyName("recipients")]
    public Optional<User[]> Recipients { get; set; }
}
