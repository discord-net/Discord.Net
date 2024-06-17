namespace Discord.Models;

public interface IInviteModel : IEntityModel
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
}
