using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord activity timestamp object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-timestamps"/>
    /// </remarks>
    public record ActivityTimestamps
    {
        /// <summary>
        /// When the <see cref="Activity"/> started.
        /// </summary>
        [JsonPropertyName("start")]
        public Optional<DateTimeOffset> Start { get; init; }

        /// <summary>
        /// When the <see cref="Activity"/> ends.
        /// </summary>
        [JsonPropertyName("end")]
        public Optional<DateTimeOffset> End { get; init; }
    }
}
