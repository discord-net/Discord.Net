using Newtonsoft.Json;

namespace Discord.API
{
    internal class PartialGuild
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("splash")]
        public Optional<string> Splash { get; set; }

        [JsonProperty("banner")]
        public Optional<string> BannerHash { get; set; }

        [JsonProperty("description")]
        public Optional<string> Description { get; set; }

        [JsonProperty("icon")]
        public Optional<string> IconHash { get; set; }

        [JsonProperty("features")]
        public Optional<GuildFeatures> Features { get; set; }

        [JsonProperty("verification_level")]
        public Optional<VerificationLevel> VerificationLevel { get; set; }

        [JsonProperty("vanity_url_code")]
        public Optional<string> VanityUrlCode { get; set; }

        [JsonProperty("premium_subscription_count")]
        public Optional<int> PremiumSubscriptionCount { get; set; }

        [JsonProperty("nsfw")]
        public Optional<bool?> Nsfw { get; set; }

        [JsonProperty("nsfw_level")]
        public Optional<NsfwLevel> NsfwLevel { get; set; }

        [JsonProperty("welcome_screen")]
        public Optional<WelcomeScreen> WelcomeScreen { get; set; }

        [JsonProperty("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; set; }

        [JsonProperty("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; set; }

    }
}
