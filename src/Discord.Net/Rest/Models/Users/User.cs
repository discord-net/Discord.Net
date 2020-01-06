#pragma warning disable CS8618 // Uninitialized NRT expected in models <username>
using System.Text.Json.Serialization;

namespace Discord.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("discriminator")]
        public ushort Discriminator { get; set; }
        [JsonPropertyName("avatar")]
        public string? AvatarId { get; set; }
        [JsonPropertyName("bot")]
        public Optional<bool> Bot { get; set; }
        [JsonPropertyName("system")]
        public Optional<bool> System { get; set; }
        [JsonPropertyName("mfa_enabled")]
        public Optional<bool> MfaEnabled { get; set; }
        [JsonPropertyName("locale")]
        public Optional<string> Locale { get; set; }
        [JsonPropertyName("verified")]
        public Optional<bool> Verified { get; set; }
        [JsonPropertyName("email")]
        public Optional<string> Email { get; set; }
        [JsonPropertyName("flags")]
        public Optional<AccountFlags> Flags { get; set; }
        [JsonPropertyName("premium_type")]
        public Optional<PremiumType> PremiumType { get; set; }
    }
}
