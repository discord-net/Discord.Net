using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a user object.
    /// </summary>
    public record User
    {
        /// <summary>
        ///     Creates a <see cref="User"/> with the provided parameters.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <param name="username">The user's username, not unique across the platform.</param>
        /// <param name="discriminator">The user's 4-digit discord-tag.</param>
        /// <param name="avatar">The user's avatar hash.</param>
        /// <param name="bot">Whether the user belongs to an OAuth2 application.</param>
        /// <param name="system">Whether the user is an Official Discord System user (part of the urgent message system).</param>
        /// <param name="mfaEnabled">Whether the user has two factor enabled on their account.</param>
        /// <param name="locale">The user's chosen language option.</param>
        /// <param name="verified">Whether the email on this account has been verified.</param>
        /// <param name="email">The user's email.</param>
        /// <param name="flags">The flags on a user's account.</param>
        /// <param name="premiumType">The type of Nitro subscription on a user's account.</param>
        /// <param name="publicFlags">The public flags on a user's account.</param>
        [JsonConstructor]
        public User(Snowflake id, string username, string discriminator, string? avatar, Optional<bool> bot, Optional<bool> system, Optional<bool> mfaEnabled, Optional<string> locale, Optional<bool> verified, Optional<string?> email, Optional<UserFlags> flags, Optional<PremiumType> premiumType, Optional<UserFlags> publicFlags)
        {
            Id = id;
            Username = username;
            Discriminator = discriminator;
            Avatar = avatar;
            Bot = bot;
            System = system;
            MfaEnabled = mfaEnabled;
            Locale = locale;
            Verified = verified;
            Email = email;
            Flags = flags;
            PremiumType = premiumType;
            PublicFlags = publicFlags;
        }

        /// <summary>
        ///     The user's id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     The user's username, not unique across the platform.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; }

        /// <summary>
        ///     The user's 4-digit discord-tag.
        /// </summary>
        [JsonPropertyName("discriminator")]
        public string Discriminator { get; }

        /// <summary>
        ///     The user's avatar hash.
        /// </summary>
        [JsonPropertyName("avatar")]
        public string? Avatar { get; }

        /// <summary>
        ///     Whether the user belongs to an OAuth2 application.
        /// </summary>
        [JsonPropertyName("bot")]
        public Optional<bool> Bot { get; }

        /// <summary>
        ///     Whether the user is an Official Discord System user (part of the urgent message system).
        /// </summary>
        [JsonPropertyName("system")]
        public Optional<bool> System { get; }

        /// <summary>
        ///     Whether the user has two factor enabled on their account.
        /// </summary>
        [JsonPropertyName("mfa_enabled")]
        public Optional<bool> MfaEnabled { get; }

        /// <summary>
        ///     The user's chosen language option.
        /// </summary>
        [JsonPropertyName("locale")]
        public Optional<string> Locale { get; }

        /// <summary>
        ///     Whether the email on this account has been verified.
        /// </summary>
        [JsonPropertyName("verified")]
        public Optional<bool> Verified { get; }

        /// <summary>
        ///     The user's email.
        /// </summary>
        [JsonPropertyName("email")]
        public Optional<string?> Email { get; }

        /// <summary>
        ///     The flags on a user's account.
        /// </summary>
        [JsonPropertyName("flags")]
        public Optional<UserFlags> Flags { get; }

        /// <summary>
        ///     The type of Nitro subscription on a user's account.
        /// </summary>
        [JsonPropertyName("premium_type")]
        public Optional<PremiumType> PremiumType { get; }

        /// <summary>
        ///     The public flags on a user's account.
        /// </summary>
        [JsonPropertyName("public_flags")]
        public Optional<UserFlags> PublicFlags { get; }
    }
}
