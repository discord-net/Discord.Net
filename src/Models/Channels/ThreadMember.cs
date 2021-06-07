using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord thread member object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#thread-member-object-thread-member-structure"/>
    /// </remarks>
    public record ThreadMember
    {
        /// <summary>
        /// The id of the thread.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// The id of the <see cref="User"/>.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; init; }

        /// <summary>
        /// The time the current <see cref="User"/> last joined the thread.
        /// </summary>
        [JsonPropertyName("join_timestamp")]
        public DateTimeOffset JoinTimestamp { get; init; }

        /// <summary>
        /// Any user-thread settings, currently only used for notifications.
        /// </summary>
        [JsonPropertyName("flags")]
        public int Flags { get; init; }
    }
}
