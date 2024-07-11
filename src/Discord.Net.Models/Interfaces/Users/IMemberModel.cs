namespace Discord.Models;

[ModelEquality]
public partial interface IMemberModel : IEntityModel<ulong>
{
    int Flags { get; }
    string? Nickname { get;}
    string? Avatar { get;}
    ulong[] RoleIds { get;}
    DateTimeOffset? JoinedAt { get;}
    DateTimeOffset? PremiumSince { get;}
    bool? IsPending { get;}
    DateTimeOffset? CommunicationsDisabledUntil { get; }
}
