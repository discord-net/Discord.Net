using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Team
{
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("members")]
    public required TeamMember[] TeamMembers { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("owner_user_id")]
    public ulong OwnerUserId { get; set; }
}
