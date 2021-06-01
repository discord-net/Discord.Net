using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a thread member object.
    /// </summary>
    public record ThreadMember
    {
        /// <summary>
        ///     Creates a <see cref="ThreadMember"/> with the provided parameters.
        /// </summary>
        /// <param name="id">The id of the thread.</param>
        /// <param name="userId">The id of the user.</param>
        /// <param name="joinTimestamp">The time the current user last joined the thread.</param>
        /// <param name="flags">Any user-thread settings, currently only used for notifications.</param>
        [JsonConstructor]
        public ThreadMember(Snowflake id, Snowflake userId, DateTimeOffset joinTimestamp, int flags)
        {
            Id = id;
            UserId = userId;
            JoinTimestamp = joinTimestamp;
            Flags = flags;
        }

        /// <summary>
        ///     The id of the thread.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     The id of the user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; }

        /// <summary>
        ///     The time the current user last joined the thread.
        /// </summary>
        [JsonPropertyName("join_timestamp")]
        public DateTimeOffset JoinTimestamp { get; }

        /// <summary>
        ///     Any user-thread settings, currently only used for notifications.
        /// </summary>
        [JsonPropertyName("flags")]
        public int Flags { get; }
    }
}
