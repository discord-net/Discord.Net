using System.Text.Json.Serialization;

namespace Discord.API;

internal class MessageMentionedChannel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("type")]
    public ChannelType ChannelType { get; set; }

    [JsonPropertyName("name")]
    public string ChannelName { get; set; }
}
