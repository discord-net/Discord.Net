namespace Discord.Models;

public interface IPartialGuildModel : IEntityModel<ulong>
{
    string Name { get; }
    string? SplashId { get; }
    string? BannerId { get; }
    string? Description { get; }
    string? IconId { get; }
    string[]? Features { get; }
    int? VerificationLevel { get; }
    string? VanityUrlCode { get; }
    int? NsfwLevel { get; }
    int? PremiumSubscriptionCount { get; }
    int? ApproximateMemberCount { get; }
    int? ApproximatePresenceCount { get; }
}
