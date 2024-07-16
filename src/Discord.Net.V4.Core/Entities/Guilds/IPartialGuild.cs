using Discord.Models;

namespace Discord;

public interface IPartialGuild : ISnowflakeEntity<IPartialGuildModel>
{
    string Name { get; }
    string? SplashId { get; }
    string? BannerId { get; }
    string? Description { get; }
    string? IconId { get; }
    GuildFeatures? Features { get; }
    VerificationLevel? VerificationLevel { get; }
    string? VanityUrlCode { get; }
    NsfwLevel? NsfwLevel { get; }
    int? PremiumSubscriptionCount { get; }
}
