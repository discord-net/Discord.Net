using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord guild member object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-member-object-guild-member-structure"/>
    /// </remarks>
    public record GuildMember
    {
        /// <summary>
        /// The <see cref="Models.User"/> this <see cref="GuildMember"/> represents.
        /// </summary>
        [JsonPropertyName("user")]
        public Optional<User> User { get; init; }

        /// <summary>
        /// This users <see cref="Guild"/> nickname.
        /// </summary>
        [JsonPropertyName("nick")]
        public Optional<string?> Nick { get; init; }

        /// <summary>
        /// Array of <see cref="Role"/> ids.
        /// </summary>
        [JsonPropertyName("roles")]
        public Snowflake[]? Roles { get; init; } // Required property candidate

        /// <summary>
        /// When the user joined the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("joined_at")]
        public DateTimeOffset JoinedAt { get; init; }

        /// <summary>
        /// When the user started boosting the <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("premium_since")]
        public Optional<DateTimeOffset?> PremiumSince { get; init; }

        /// <summary>
        /// Whether the user is deafened in voice <see cref="Channel"/>s.
        /// </summary>
        [JsonPropertyName("deaf")]
        public bool Deaf { get; init; }

        /// <summary>
        /// Whether the user is muted in voice <see cref="Channel"/>s.
        /// </summary>
        [JsonPropertyName("mute")]
        public bool Mute { get; init; }

        /// <summary>
        /// Whether the user has not yet passed the <see cref="Guild"/>'s
        /// Membership Screening requirements.
        /// </summary>
        [JsonPropertyName("pending")]
        public Optional<bool> Pending { get; init; }

        /// <summary>
        /// Total <see cref="Models.Permissions"/> of the <see cref="GuildMember"/>
        /// in the <see cref="Channel"/>, including <see cref="Overwrite"/>s, returned
        /// when in the interaction object.
        /// </summary>
        [JsonPropertyName("permissions")]
        public Optional<Permissions> Permissions { get; init; }
    }
}
