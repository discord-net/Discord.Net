namespace Discord.Models;

public interface ISelfUserModel : IUserModel
{
    int? PremiumType { get; }
    string? Email { get; }
    bool? Verified { get; }
    string? Locale { get; }
    bool? MFAEnabled { get; }
}
