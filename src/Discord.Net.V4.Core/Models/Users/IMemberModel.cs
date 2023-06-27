namespace Discord.Models;

public interface IMemberModel : IEntityModel<ulong>
{
    string? Nickname { get; set; }
    string? GuildAvatar { get; set; }
    ulong[] Roles { get; set; }
    DateTimeOffset? JoinedAt { get; set; }
    DateTimeOffset? PremiumSince { get; set; }
    bool? IsPending { get; set; }
    DateTimeOffset? CommunicationsDisabledUntil { get; set; }
}
