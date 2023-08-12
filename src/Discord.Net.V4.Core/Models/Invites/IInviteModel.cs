namespace Discord.Models;

public interface IInviteModel 
{
    string Code { get; set; }
    IPartialGuildModel? Guild { get; set; }
    IChannelModel? Channel { get; set; }
    IUserModel? Inviter { get; set; }
    InviteTargetType? TargetType { get; set; }
    IUserModel? TargetUser { get; set; }
    IApplicationModel? TargetApplication { get; set; }

    int? ApproximatePresenceCount { get; set; }
    int? ApproximateMemberCount { get; set; }
    DateTimeOffset? ExpiresAt { get; set; }
    IGuildEventModel? ScheduledEvent { get; set; }
}
