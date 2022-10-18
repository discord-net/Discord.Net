using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class User
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("username")]
        public Optional<string> Username { get; set; }
        [JsonPropertyName("discriminator")]
        public Optional<string> Discriminator { get; set; }
        [JsonPropertyName("bot")]
        public Optional<bool> Bot { get; set; }
        [JsonPropertyName("avatar")]
        public Optional<string> Avatar { get; set; }
        [JsonPropertyName("banner")]
        public Optional<string> Banner { get; set; }
        [JsonPropertyName("accent_color")]
        public Optional<uint?> AccentColor { get; set; }

        //CurrentUser
        [JsonPropertyName("verified")]
        public Optional<bool> Verified { get; set; }
        [JsonPropertyName("email")]
        public Optional<string> Email { get; set; }
        [JsonPropertyName("mfa_enabled")]
        public Optional<bool> MfaEnabled { get; set; }
        [JsonPropertyName("flags")]
        public Optional<UserProperties> Flags { get; set; }
        [JsonPropertyName("premium_type")]
        public Optional<PremiumType> PremiumType { get; set; }
        [JsonPropertyName("locale")]
        public Optional<string> Locale { get; set; }
        [JsonPropertyName("public_flags")]
        public Optional<UserProperties> PublicFlags { get; set; }
    }
}
