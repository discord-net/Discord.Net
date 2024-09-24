using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class BulkBanResponse
{
    [JsonPropertyName("banned_users")]
    public required ulong[] BannedUsers { get; set; }

    [JsonPropertyName("failed_users")]
    public required ulong[] FailedUsers { get; set; }
}
