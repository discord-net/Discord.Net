namespace Discord.Models;

public interface IInviteCreatedPayloadData : IGatewayPayloadData
{
    ulong ChannelId { get; }
    string Code { get; }
    DateTimeOffset CreatedAt { get; }
    ulong? GuildId { get; }
    IUserModel? Inviter { get; }
    int MaxAge { get; }
    int MaxUses { get; }
    int? TargetType { get; }
    IUserModel? TargetUser { get; }
    IApplicationModel? TargetApplication { get; }
    bool IsTemporary { get; }
    int Uses { get; }
}
