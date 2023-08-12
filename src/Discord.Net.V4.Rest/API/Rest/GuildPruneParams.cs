using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class GuildPruneParams
{
    [JsonPropertyName("days")]
    public int Days { get; set; }

    [JsonPropertyName("include_roles")]
    public ulong[] IncludeRoleIds { get; set; }
}
