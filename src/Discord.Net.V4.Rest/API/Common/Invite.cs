using System.Text.Json.Serialization;

namespace Discord.API;

internal class Invite
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("guild")]
    public Optional<PartialGuild> Guild { get; set; }

    [JsonPropertyName("channel")]
    public Channel? Channel { get; set; }

    [JsonPropertyName("inviter")]
    public Optional<User> Inviter { get; set; }

    [JsonPropertyName("target_type")]
    public Optional<TargetUserType> TargetUserType { get; set; }

    [JsonPropertyName("target_user")]
    public Optional<User> TargetUser { get; set; }

    [JsonPropertyName("target_application")]
    public Optional<Application> Application { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public Optional<int> PresenceCount { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public Optional<int> MemberCount { get; set; }

    [JsonPropertyName("expires_at")]
    public Optional<DateTimeOffset?> ExpiresAt { get; set; }

    [JsonPropertyName("guild_scheduled_event")]
    public Optional<GuildScheduledEvent> ScheduledEvent { get; set; }
}
