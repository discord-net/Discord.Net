using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class PartialGuild
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
    public Optional<GuildFeatures> Features { get; set; }

    [JsonPropertyName("verification_level")]
    public Optional<VerificationLevel> VerificationLevel { get; set; }

    [JsonPropertyName("vanity_url_code")]
    public Optional<string?> VanityUrlCode { get; set; }

    [JsonPropertyName("premium_subscription_count")]
    public Optional<int> PremiumSubscriptionCount { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool?> Nsfw { get; set; }

    [JsonPropertyName("nsfw_level")]
    public Optional<NsfwLevel> NsfwLevel { get; set; }

    [JsonPropertyName("welcome_screen")]
    public Optional<WelcomeScreen> WelcomeScreen { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public Optional<int> ApproximateMemberCount { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public Optional<int> ApproximatePresenceCount { get; set; }

}
