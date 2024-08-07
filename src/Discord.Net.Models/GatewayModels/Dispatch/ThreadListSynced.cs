using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ThreadListSynced : IGatewayPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_ids")]
    public Optional<ulong[]> ChannelIds { get; set; }

    [JsonPropertyName("threads")]
    public required ThreadChannelBase[] Threads { get; set; }

    [JsonPropertyName("members")]
    public required ThreadMember[] Members { get; set; }
}
