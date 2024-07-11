namespace Discord.Models;

[ModelEquality]
public partial interface IInviteModel : IEntityModel<string>
{
    string Code { get; }
    ulong? GuildId { get; }
    ulong? ChannelId { get; }
    ulong? InviterId { get; }
    int? TargetType { get; }
    ulong? TargetUserId { get; }
    IApplicationModel? TargetApplication { get; }

    int? ApproximatePresenceCount { get; }
    int? ApproximateMemberCount { get; }
    DateTimeOffset? ExpiresAt { get; }
    ulong? ScheduledEventId { get; }

    string IEntityModel<string>.Id => Code;
}
