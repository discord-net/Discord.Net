using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord invite object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/invite#invite-object-invite-structure"/>
    /// </remarks>
    public record Invite
    {
        /// <summary>
        /// The <see cref="Invite"/> code (unique ID).
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; init; } // Required property candidate

        /// <summary>
        /// The <see cref="Guild"/> this <see cref="Invite"/> is for.
        /// </summary>
        [JsonPropertyName("guild")]
        public Optional<Guild> Guild { get; init; }

        /// <summary>
        /// The <see cref="Models.Channel"/> this invite is for.
        /// </summary>
        [JsonPropertyName("channel")]
        public Channel? Channel { get; init; } // Required property candidate

        /// <summary>
        /// The <see cref="User"/> who created the <see cref="Invite"/>.
        /// </summary>
        [JsonPropertyName("inviter")]
        public Optional<User> Inviter { get; init; }

        /// <summary>
        /// The type of target for this voice <see cref="Models.Channel"/> <see cref="Invite"/>.
        /// </summary>
        [JsonPropertyName("target_type")]
        public Optional<InviteTargetType> TargetType { get; init; }

        /// <summary>
        /// The <see cref="User"/> whose stream to display for this voice <see cref="Models.Channel"/> stream <see cref="Invite"/>.
        /// </summary>
        [JsonPropertyName("target_user")]
        public Optional<User> TargetUser { get; init; }

        /// <summary>
        /// The embedded <see cref="Application"/> to open for this voice <see cref="Models.Channel"/> embedded <see cref="Application"/> <see cref="Invite"/>.
        /// </summary>
        [JsonPropertyName("target_application")]
        public Optional<Application> TargetApplication { get; init; }

        /// <summary>
        /// Approximate count of online <see cref="GuildMember"/>s.
        /// </summary>
        [JsonPropertyName("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; init; }

        /// <summary>
        /// Approximate count of total <see cref="GuildMember"/>.
        /// </summary>
        [JsonPropertyName("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; init; }

        /// <summary>
        /// The expiration date of this <see cref="Invite"/>.
        /// </summary>
        [JsonPropertyName("expires_at")]
        public Optional<DateTimeOffset?> ExpiresAt { get; init; }
    }
}
