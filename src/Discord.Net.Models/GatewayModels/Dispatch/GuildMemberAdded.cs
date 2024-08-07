using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class GuildMemberAdded : IGuildMemberAddedPayloadData
{
    [JsonIgnore, JsonExtend]
    public required GuildMember Member { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}
