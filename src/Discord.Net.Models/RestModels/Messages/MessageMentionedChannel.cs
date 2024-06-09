using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MessageMentionedChannel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("type")]
    public int ChannelType { get; set; }

    [JsonPropertyName("name")]
    public required string ChannelName { get; set; }
}
