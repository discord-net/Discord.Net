using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a invite object.
    /// </summary>
    public record Invite
    {
        /// <summary>
        ///     Creates a <see cref="Invite"/> with the provided parameters.
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
        [JsonConstructor]
        public Invite(string code, Optional<Guild> guild, Channel channel, Optional<User> inviter, Optional<InviteTargetType> targetType, Optional<User> targetUser, Optional<Application> targetApplication, Optional<int> approximatePresenceCount, Optional<int> approximateMemberCount, Optional<DateTimeOffset?> expiresAt)
        {
            Code = code;
            Guild = guild;
            Channel = channel;
            Inviter = inviter;
            TargetType = targetType;
            TargetUser = targetUser;
            TargetApplication = targetApplication;
            ApproximatePresenceCount = approximatePresenceCount;
            ApproximateMemberCount = approximateMemberCount;
            ExpiresAt = expiresAt;
        }

        /// <summary>
        ///     The invite code (unique ID).
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; }

        /// <summary>
        ///     The guild this invite is for.
        /// </summary>
        [JsonPropertyName("guild")]
        public Optional<Guild> Guild { get; }

        /// <summary>
        ///     The channel this invite is for.
        /// </summary>
        [JsonPropertyName("channel")]
        public Channel Channel { get; }

        /// <summary>
        ///     The user who created the invite.
        /// </summary>
        [JsonPropertyName("inviter")]
        public Optional<User> Inviter { get; }

        /// <summary>
        ///     The type of target for this voice channel invite.
        /// </summary>
        [JsonPropertyName("target_type")]
        public Optional<InviteTargetType> TargetType { get; }

        /// <summary>
        ///     The user whose stream to display for this voice channel stream invite.
        /// </summary>
        [JsonPropertyName("target_user")]
        public Optional<User> TargetUser { get; }

        /// <summary>
        ///     The embedded application to open for this voice channel embedded application invite.
        /// </summary>
        [JsonPropertyName("target_application")]
        public Optional<Application> TargetApplication { get; }

        /// <summary>
        ///     Approximate count of online members.
        /// </summary>
        [JsonPropertyName("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; }

        /// <summary>
        ///     Approximate count of total members.
        /// </summary>
        [JsonPropertyName("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; }

        /// <summary>
        ///     The expiration date of this invite.
        /// </summary>
        [JsonPropertyName("expires_at")]
        public Optional<DateTimeOffset?> ExpiresAt { get; }
    }
}
