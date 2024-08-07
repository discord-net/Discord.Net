using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class BulkMessageDeleted : IBulkMessageDeletePayloadData
{
    [JsonPropertyName("ids")]
    public required ulong[] Ids { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_Id")]
    public Optional<ulong> GuildId { get; set; }

    ulong? IBulkMessageDeletePayloadData.GuildId => GuildId.ToNullable();
}
