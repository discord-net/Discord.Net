using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ThreadMembersUpdated : IGatewayPayloadData
{
    [JsonPropertyName("id")]
    public ulong ThreadId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("member_count")]
    public int MemberCount { get; set; }

    [JsonPropertyName("added_members")]
    public Optional<ThreadMember[]> AddedMembers { get; set; }

    [JsonPropertyName("removed_member_ids")]
    public Optional<ulong[]> RemovedMembers { get; set; }
}
