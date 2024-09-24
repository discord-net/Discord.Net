using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MessageMentionedChannel : IMentionedChannelModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
