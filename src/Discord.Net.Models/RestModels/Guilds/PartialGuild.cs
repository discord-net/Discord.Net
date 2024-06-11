using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class PartialGuild : IPartialGuildModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("splash")]
    public Optional<string?> Splash { get; set; }

    [JsonPropertyName("banner")]
    public Optional<string?> BannerHash { get; set; }

    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }

    [JsonPropertyName("icon")]
    public Optional<string?> IconHash { get; set; }

    [JsonPropertyName("features")]
    public Optional<string[]> Features { get; set; }

    [JsonPropertyName("verification_level")]
    public Optional<int> VerificationLevel { get; set; }

    [JsonPropertyName("vanity_url_code")]
    public Optional<string?> VanityUrlCode { get; set; }

    [JsonPropertyName("premium_subscription_count")]
    public Optional<int> PremiumSubscriptionCount { get; set; }

    [JsonPropertyName("nsfw_level")]
    public Optional<int> NsfwLevel { get; set; }

    [JsonPropertyName("welcome_screen")]
    public Optional<WelcomeScreen> WelcomeScreen { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public Optional<int> ApproximateMemberCount { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public Optional<int> ApproximatePresenceCount { get; set; }

    string? IPartialGuildModel.SplashId => Splash;

    string? IPartialGuildModel.BannerId => BannerHash;

    string? IPartialGuildModel.Description => Description;

    string? IPartialGuildModel.IconId => IconHash;

    string[]? IPartialGuildModel.Features => Features;
    int? IPartialGuildModel.VerificationLevel => VerificationLevel.ToNullable();
    string? IPartialGuildModel.VanityUrlCode => VanityUrlCode;
    int? IPartialGuildModel.NsfwLevel => NsfwLevel;
    int? IPartialGuildModel.PremiumSubscriptionCount => PremiumSubscriptionCount;

}
