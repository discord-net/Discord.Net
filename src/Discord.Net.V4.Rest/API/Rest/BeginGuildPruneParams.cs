using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class BeginGuildPruneParams
{
    [JsonPropertyName("days")]
    public int Days { get; set; }

    [JsonPropertyName("compute_prune_count")]
    public bool ComputePruneCount { get; set; }

    [JsonPropertyName("include_roles")]
    public ulong[] IncludeRoleIds { get; set; }
}
