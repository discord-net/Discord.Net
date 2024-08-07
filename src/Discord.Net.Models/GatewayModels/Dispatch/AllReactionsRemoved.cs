using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AllReactionsRemoved : IMessageReactionRemoveAllPayloadData
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    ulong? IMessageReactionRemoveAllPayloadData.GuildId => GuildId.ToNullable();
}
