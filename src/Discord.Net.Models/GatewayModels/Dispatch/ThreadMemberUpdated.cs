using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ThreadMemberUpdated : IGatewayPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonIgnore, JsonExtend]
    public required ThreadMember Member { get; set; }
}
