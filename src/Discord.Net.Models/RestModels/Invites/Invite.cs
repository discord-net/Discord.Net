using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class Invite :
    IInviteModel,
    IModelSource,
    IModelSourceOf<IPartialGuildModel?>,
    IModelSourceOfMultiple<IUserModel>,
    IModelSourceOf<IApplicationModel?>,
    IModelSourceOf<IGuildScheduledEventModel?>
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("guild")]
    public Optional<PartialGuild> Guild { get; set; }

    [JsonPropertyName("channel")]
    public ChannelModel? Channel { get; set; }

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

    public IEnumerable<IEntityModel> GetDefinedModels()
    {
        if (Channel is not null)
            yield return Channel;

        if (TargetUser.IsSpecified)
            yield return TargetUser.Value;

        if (Inviter.IsSpecified)
            yield return Inviter.Value;

        if (TargetApplication.IsSpecified)
            yield return TargetApplication.Value;

        if (ScheduledEvent.IsSpecified)
            yield return ScheduledEvent.Value;
    }

    IEnumerable<IUserModel> IModelSourceOfMultiple<IUserModel>.GetModels()
    {
        if (TargetUser.IsSpecified)
            yield return TargetUser.Value;

        if (Inviter.IsSpecified)
            yield return Inviter.Value;
    }

    IPartialGuildModel? IModelSourceOf<IPartialGuildModel?>.Model => ~Guild;

    IApplicationModel? IModelSourceOf<IApplicationModel?>.Model => ~TargetApplication;

    IGuildScheduledEventModel? IModelSourceOf<IGuildScheduledEventModel?>.Model => ~ScheduledEvent;
}
