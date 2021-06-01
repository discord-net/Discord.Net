using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///      Represents a user mention object.
    /// </summary>
    public record UserMention : User
    {
        /// <summary>
        ///     Creates a <see cref="UserMention"/> with the provided parameters.
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
        /// <param name="member">Additional partial member field.</param>
        [JsonConstructor]
        public UserMention(Snowflake id, string username, string discriminator, string? avatar, Optional<bool> bot, Optional<bool> system, Optional<bool> mfaEnabled, Optional<string> locale, Optional<bool> verified, Optional<string?> email, Optional<UserFlags> flags, Optional<PremiumType> premiumType, Optional<UserFlags> publicFlags, Optional<GuildMember> member)
            : base(id, username, discriminator, avatar, bot, system, mfaEnabled, locale, verified, email, flags, premiumType, publicFlags)
        {
            Member = member;
        }

        /// <summary>
        ///     Additional partial member field.
        /// </summary>
        [JsonPropertyName("member")]
        public Optional<GuildMember> Member { get; }
    }
}
