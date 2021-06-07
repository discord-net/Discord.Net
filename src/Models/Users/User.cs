using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord user object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/user#user-object-user-structure"/>
    /// </remarks>
    public record User
    {
        /// <summary>
        /// The <see cref="User"/>'s id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// The <see cref="User"/>'s username, not unique across the platform.
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; init; } // Required property candidate

        /// <summary>
        /// The <see cref="User"/>'s 4-digit discord-tag.
        /// </summary>
        [JsonPropertyName("discriminator")]
        public string? Discriminator { get; init; } // Required property candidate

        /// <summary>
        /// The <see cref="User"/>'s avatar hash.
        /// </summary>
        [JsonPropertyName("avatar")]
        public string? Avatar { get; init; }

        /// <summary>
        /// Whether the <see cref="User"/> belongs to an OAuth2 <see cref="Application"/>.
        /// </summary>
        [JsonPropertyName("bot")]
        public Optional<bool> Bot { get; init; }

        /// <summary>
        /// Whether the <see cref="User"/> is an Official Discord System user (part of the urgent message system).
        /// </summary>
        [JsonPropertyName("system")]
        public Optional<bool> System { get; init; }

        /// <summary>
        /// Whether the <see cref="User"/> has two factor enabled on their account.
        /// </summary>
        [JsonPropertyName("mfa_enabled")]
        public Optional<bool> MfaEnabled { get; init; }

        /// <summary>
        /// The <see cref="User"/>'s chosen language option.
        /// </summary>
        [JsonPropertyName("locale")]
        public Optional<string> Locale { get; init; }

        /// <summary>
        /// Whether the email on this account has been verified.
        /// </summary>
        [JsonPropertyName("verified")]
        public Optional<bool> Verified { get; init; }

        /// <summary>
        /// The <see cref="User"/>'s email.
        /// </summary>
        [JsonPropertyName("email")]
        public Optional<string?> Email { get; init; }

        /// <summary>
        /// The flags on a <see cref="User"/>'s account.
        /// </summary>
        [JsonPropertyName("flags")]
        public Optional<UserFlags> Flags { get; init; }

        /// <summary>
        /// The type of Nitro subscription on a <see cref="User"/>'s account.
        /// </summary>
        [JsonPropertyName("premium_type")]
        public Optional<PremiumType> PremiumType { get; init; }

        /// <summary>
        /// The public flags on a <see cref="User"/>'s account.
        /// </summary>
        [JsonPropertyName("public_flags")]
        public Optional<UserFlags> PublicFlags { get; init; }
    }
}
