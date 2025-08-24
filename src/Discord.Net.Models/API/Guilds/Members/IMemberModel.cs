using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMemberModel : IModel
{
    Optional<IdOrModel<Snowflake, IUserModel>> User { get; }
    
    Optional<string?> Nick { get; }
    
    Optional<string?> Avatar { get; }
    
    Optional<string?> Banner { get; }
    
    IReadOnlyList<Snowflake> Roles { get; }
    
    DateTimeOffset? JoinedAt { get; }
    
    Optional<DateTimeOffset?> PremiumSince { get; }
    
    bool Deaf { get; }
    
    bool Mute { get; }
    
    MemberFlags Flags { get; }
    
    Optional<bool> Pending { get; }
    
    Optional<PermissionBitSet> Permissions { get; }
    
    Optional<DateTimeOffset?> CommunicationsDisabledUntil { get; }
    
    Optional<IAvatarDecorationDataModel?> AvatarDecorationData { get; }
}