using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a guild member object.
    /// </summary>
    public record GuildMember
    {
        /// <summary>
        ///     Creates a <see cref="GuildMember"/> with the provided parameters.
        /// </summary>
        /// <param name="user">The user this guild member represents.</param>
        /// <param name="nick">This users guild nickname.</param>
        /// <param name="roles">Array of role object ids.</param>
        /// <param name="joinedAt">When the user joined the guild.</param>
        /// <param name="premiumSince">When the user started boosting the guild.</param>
        /// <param name="deaf">Whether the user is deafened in voice channels.</param>
        /// <param name="mute">Whether the user is muted in voice channels.</param>
        /// <param name="pending">Whether the user has not yet passed the guild's Membership Screening requirements.</param>
        /// <param name="permissions">Total permissions of the member in the channel, including overwrites, returned when in the interaction object.</param>
        [JsonConstructor]
        public GuildMember(Optional<User> user, Optional<string?> nick, Snowflake[] roles, DateTimeOffset joinedAt, Optional<DateTimeOffset?> premiumSince, bool deaf, bool mute, Optional<bool> pending, Optional<Permissions> permissions)
        {
            User = user;
            Nick = nick;
            Roles = roles;
            JoinedAt = joinedAt;
            PremiumSince = premiumSince;
            Deaf = deaf;
            Mute = mute;
            Pending = pending;
            Permissions = permissions;
        }

        /// <summary>
        ///     The user this guild member represents.
        /// </summary>
        [JsonPropertyName("user")]
        public Optional<User> User { get; }

        /// <summary>
        ///     This users guild nickname.
        /// </summary>
        [JsonPropertyName("nick")]
        public Optional<string?> Nick { get; }

        /// <summary>
        ///     Array of role object ids.
        /// </summary>
        [JsonPropertyName("roles")]
        public Snowflake[] Roles { get; }

        /// <summary>
        ///     When the user joined the guild.
        /// </summary>
        [JsonPropertyName("joined_at")]
        public DateTimeOffset JoinedAt { get; }

        /// <summary>
        ///     When the user started boosting the guild.
        /// </summary>
        [JsonPropertyName("premium_since")]
        public Optional<DateTimeOffset?> PremiumSince { get; }

        /// <summary>
        ///     Whether the user is deafened in voice channels.
        /// </summary>
        [JsonPropertyName("deaf")]
        public bool Deaf { get; }

        /// <summary>
        ///     Whether the user is muted in voice channels.
        /// </summary>
        [JsonPropertyName("mute")]
        public bool Mute { get; }

        /// <summary>
        ///     Whether the user has not yet passed the guild's Membership Screening requirements.
        /// </summary>
        [JsonPropertyName("pending")]
        public Optional<bool> Pending { get; }

        /// <summary>
        ///     Total permissions of the member in the channel, including overwrites, returned when in the interaction object.
        /// </summary>
        [JsonPropertyName("permissions")]
        public Optional<Permissions> Permissions { get; }
    }
}
