namespace Discord.Models;

public interface IMemberModel : IEntityModel<ulong>
{
    GuildUserFlags Flags { get; }
    string? Nickname { get;}
    string? GuildAvatar { get;}
    ulong[] RoleIds { get;}
    DateTimeOffset? JoinedAt { get;}
    DateTimeOffset? PremiumSince { get;}
    bool? IsPending { get;}
    DateTimeOffset? CommunicationsDisabledUntil { get; }
}
