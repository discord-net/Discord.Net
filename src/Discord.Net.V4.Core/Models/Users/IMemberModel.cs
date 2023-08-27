namespace Discord.Models;

public interface IMemberModel : IEntityModel<ulong>
{
    GuildUserFlags Flags { get; set; }
    string? Nickname { get; set; }
    string? GuildAvatar { get; set; }
    ICollection<ulong> RoleIds { get; set; }
    DateTimeOffset? JoinedAt { get; set; }
    DateTimeOffset? PremiumSince { get; set; }
    bool? IsPending { get; set; }
    DateTimeOffset? CommunicationsDisabledUntil { get; set; }
}
