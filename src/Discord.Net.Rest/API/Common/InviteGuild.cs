using Newtonsoft.Json;

namespace Discord.API
{
    internal class InviteGuild
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
        public GuildFeatures Features { get; set; }

        [JsonProperty("verification_level")]
        public VerificationLevel VerificationLevel { get; set; }

        [JsonProperty("vanity_url_code")]
        public Optional<string> VanityUrlCode { get; set; }

        [JsonProperty("premium_subscription_count")]
        public Optional<int> PremiumSubscriptionCount { get; set; }

        [JsonProperty("nsfw")]
        public Optional<bool?> Nsfw { get; set; }

        [JsonProperty("nsfw_level")]
        public NsfwLevel NsfwLevel { get; set; }

        [JsonProperty("welcome_screen")]
        public Optional<WelcomeScreen> WelcomeScreen { get; set; }
    }
}
