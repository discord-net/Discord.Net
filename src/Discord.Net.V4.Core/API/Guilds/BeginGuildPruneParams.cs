using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class BeginGuildPruneParams
{
    [JsonPropertyName("days")]
    public Optional<int> Days { get; set; }

    [JsonPropertyName("compute_prune_count")]
    public Optional<bool> ComputePruneCount { get; set; }

    [JsonPropertyName("include_roles")]
    public Optional<ulong[]> IncludeRoleIds { get; set; }

    [JsonPropertyName("reason")]
    public Optional<string> Reason { get; set; }
}
