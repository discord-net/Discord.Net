using System.Text.Json.Serialization;

namespace Discord.API;

internal class TeamMember
{
    [JsonPropertyName("membership_state")]
    public MembershipState MembershipState { get; set; }

    [JsonPropertyName("permissions")]
    public string[] Permissions { get; set; }

    [JsonPropertyName("team_id")]
    public ulong TeamId { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }
}