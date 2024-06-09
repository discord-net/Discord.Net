using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class TeamMember
{
    [JsonPropertyName("membership_state")]
    public required int MembershipState { get; set; }

    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("team_id")]
    public required ulong TeamId { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }
}
