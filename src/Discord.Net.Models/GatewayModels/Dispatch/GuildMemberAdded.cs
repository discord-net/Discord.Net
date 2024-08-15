using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class GuildMemberAdded : IGuildMemberAddedPayloadData
{
    [JsonIgnore, JsonExtend] public GuildMember Member { get; set; } = null!;

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}
