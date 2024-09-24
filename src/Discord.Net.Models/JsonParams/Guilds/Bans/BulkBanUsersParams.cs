using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class BulkBanUsersParams
{
    [JsonPropertyName("user_ids")]
    public required ulong[] UserIds { get; set; }
    public Optional<int> DeleteMessageSeconds { get; set; }
}
