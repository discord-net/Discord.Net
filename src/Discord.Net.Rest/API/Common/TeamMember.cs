using Newtonsoft.Json;

namespace Discord.API;

internal class TeamMember
{
    [JsonProperty("membership_state")]
    public MembershipState MembershipState { get; set; }

    [JsonProperty("permissions")]
    public string[] Permissions { get; set; }

    [JsonProperty("team_id")]
    public ulong TeamId { get; set; }

    [JsonProperty("user")]
    public User User { get; set; }

    [JsonProperty("role")]
    public string Role { get; set; }
}
