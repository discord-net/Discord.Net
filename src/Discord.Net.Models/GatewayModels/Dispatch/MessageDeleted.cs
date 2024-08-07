using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MessageDeleted : IMessageDeletePayloadData
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    ulong? IMessageDeletePayloadData.GuildId => GuildId.ToNullable();
}
