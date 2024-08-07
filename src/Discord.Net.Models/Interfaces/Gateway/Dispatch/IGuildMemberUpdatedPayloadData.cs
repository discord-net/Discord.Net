namespace Discord.Models;

public interface IGuildMemberUpdatedPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    ulong[] Roles { get; }
    IUserModel User { get; }
    string? Nickname { get; }
    string? Avatar { get; }
    DateTimeOffset? JoinedAt { get; }
    DateTimeOffset? PremiumSince { get; }
    bool? IsDeaf { get; }
    bool? IsMute { get; }
    bool? IsPending { get; }
    DateTimeOffset? CommunicationsDisabledUntil { get; }
    int? Flags { get; }
    IAvatarDecorationDataModel? AvatarDecoration { get; }
}
