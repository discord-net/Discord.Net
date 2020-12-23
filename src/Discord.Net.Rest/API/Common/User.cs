#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class User
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("username")]
        public Optional<string> Username { get; set; }
        [JsonProperty("discriminator")]
        public Optional<string> Discriminator { get; set; }
        [JsonProperty("bot")]
        public Optional<bool> Bot { get; set; }
        [JsonProperty("avatar")]
        public Optional<string> Avatar { get; set; }

        //CurrentUser
        [JsonProperty("verified")]
        public Optional<bool> Verified { get; set; }
        [JsonProperty("email")]
        public Optional<string> Email { get; set; }
        [JsonProperty("mfa_enabled")]
        public Optional<bool> MfaEnabled { get; set; }
        [JsonProperty("flags")]
        public Optional<UserProperties> Flags { get; set; }
        [JsonProperty("premium_type")]
        public Optional<PremiumType> PremiumType { get; set; }
        [JsonProperty("locale")]
        public Optional<string> Locale { get; set; }
        [JsonProperty("public_flags")]
        public Optional<UserProperties> PublicFlags { get; set; }
    }
}
