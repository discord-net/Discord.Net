using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class Invite : IInviteModel
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("guild")]
    public Optional<PartialGuild> Guild { get; set; }

    [JsonPropertyName("channel")]
    public Channel? Channel { get; set; }

    [JsonPropertyName("inviter")]
    public Optional<User> Inviter { get; set; }

    [JsonPropertyName("target_type")]
    public Optional<int> TargetType { get; set; }

    [JsonPropertyName("target_user")]
    public Optional<User> TargetUser { get; set; }

    [JsonPropertyName("target_application")]
    public Optional<Application> TargetApplication { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public Optional<int> ApproximatePresenceCount { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public Optional<int> ApproximateMemberCount { get; set; }

    [JsonPropertyName("expires_at")]
    public Optional<DateTimeOffset?> ExpiresAt { get; set; }

    [JsonPropertyName("guild_scheduled_event")]
    public Optional<GuildScheduledEvent> ScheduledEvent { get; set; }

    ulong? IInviteModel.GuildId => Guild.Map(v => v.Id);

    ulong? IInviteModel.ChannelId => Channel?.Id;

    ulong? IInviteModel.InviterId => Inviter.Map(v => v.Id);

    int? IInviteModel.TargetType => TargetType;

    ulong? IInviteModel.TargetUserId => TargetUser.Map(v => v.Id);

    IApplicationModel? IInviteModel.TargetApplication => ~TargetApplication;

    int? IInviteModel.ApproximatePresenceCount => ApproximatePresenceCount;

    int? IInviteModel.ApproximateMemberCount => ApproximateMemberCount;

    DateTimeOffset? IInviteModel.ExpiresAt => ExpiresAt;

    ulong? IInviteModel.ScheduledEventId => ScheduledEvent.Map(v => v.Id);
}
