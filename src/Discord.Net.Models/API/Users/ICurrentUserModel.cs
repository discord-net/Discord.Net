using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface ICurrentUserModel : IUserModel
{
    Optional<PremiumType> PremiumType { get; }
    Optional<string> Email { get; }
    Optional<bool> IsVerified { get; }
    Optional<string> Locale { get; }
    Optional<bool> MFAEnabled { get; }
}