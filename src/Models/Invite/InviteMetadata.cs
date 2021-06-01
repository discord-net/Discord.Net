using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a invite metadata object.
    /// </summary>
    public record InviteMetadata : Invite
    {
        /// <summary>
        ///     Creates a <see cref="InviteMetadata"/> with the provided parameters.
        /// </summary>
        /// <param name="code">The invite code (unique ID).</param>
        /// <param name="guild">The guild this invite is for.</param>
        /// <param name="channel">The channel this invite is for.</param>
        /// <param name="inviter">The user who created the invite.</param>
        /// <param name="targetType">The type of target for this voice channel invite.</param>
        /// <param name="targetUser">The user whose stream to display for this voice channel stream invite.</param>
        /// <param name="targetApplication">The embedded application to open for this voice channel embedded application invite.</param>
        /// <param name="approximatePresenceCount">Approximate count of online members.</param>
        /// <param name="approximateMemberCount">Approximate count of total members.</param>
        /// <param name="expiresAt">The expiration date of this invite.</param>
        /// <param name="uses">Number of times this invite has been used.</param>
        /// <param name="maxUses">Max number of times this invite can be used.</param>
        /// <param name="maxAge">Duration (in seconds) after which the invite expires.</param>
        /// <param name="temporary">Whether this invite only grants temporary membership.</param>
        /// <param name="createdAt">When this invite was created.</param>
        [JsonConstructor]
        public InviteMetadata(string code, Optional<Guild> guild, Channel channel, Optional<User> inviter, Optional<InviteTargetType> targetType, Optional<User> targetUser, Optional<Application> targetApplication, Optional<int> approximatePresenceCount, Optional<int> approximateMemberCount, Optional<DateTimeOffset?> expiresAt, int uses, int maxUses, int maxAge, bool temporary, DateTimeOffset createdAt)
            : base(code, guild, channel, inviter, targetType, targetUser, targetApplication, approximatePresenceCount, approximateMemberCount, expiresAt)
        {
            Uses = uses;
            MaxUses = maxUses;
            MaxAge = maxAge;
            Temporary = temporary;
            CreatedAt = createdAt;
        }

        /// <summary>
        ///     Number of times this invite has been used.
        /// </summary>
        [JsonPropertyName("uses")]
        public int Uses { get; }

        /// <summary>
        ///     Max number of times this invite can be used.
        /// </summary>
        [JsonPropertyName("max_uses")]
        public int MaxUses { get; }

        /// <summary>
        ///     Duration (in seconds) after which the invite expires.
        /// </summary>
        [JsonPropertyName("max_age")]
        public int MaxAge { get; }

        /// <summary>
        ///     Whether this invite only grants temporary membership.
        /// </summary>
        [JsonPropertyName("temporary")]
        public bool Temporary { get; }

        /// <summary>
        ///     When this invite was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; }
    }
}
