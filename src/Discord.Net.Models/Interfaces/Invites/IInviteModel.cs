namespace Discord.Models;

public interface IInviteModel
{
    string Code { get; set; }
    ulong? GuildId { get; set; }
    ulong? ChannelId { get; set; }
    ulong? InviterId { get; set; }
    int? TargetType { get; set; }
    ulong? TargetUserId { get; set; }
    IApplicationModel? TargetApplication { get; set; }

    int? ApproximatePresenceCount { get; set; }
    int? ApproximateMemberCount { get; set; }
    DateTimeOffset? ExpiresAt { get; set; }
    ulong? ScheduledEventId { get; set; }
}
