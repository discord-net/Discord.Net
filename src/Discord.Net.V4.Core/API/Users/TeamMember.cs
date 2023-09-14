using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class TeamMember
{
    [JsonPropertyName("membership_state")]
    public required MembershipState MembershipState { get; set; }

    [JsonPropertyName("permissions")]
    public required string[] Permissions { get; set; }

    [JsonPropertyName("team_id")]
    public required ulong TeamId { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }
}
