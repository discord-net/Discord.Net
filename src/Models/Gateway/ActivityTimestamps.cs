using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an activity timestamp object.
    /// </summary>
    public record ActivityTimestamps
    {
        /// <summary>
        ///     Creates a <see cref="ActivityTimestamps"/> with the provided parameters.
        /// </summary>
        /// <param name="start">When the activity started.</param>
        /// <param name="end">When the activity ends.</param>
        [JsonConstructor]
        public ActivityTimestamps(Optional<DateTimeOffset> start, Optional<DateTimeOffset> end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        ///     When the activity started.
        /// </summary>
        [JsonPropertyName("start")]
        public Optional<DateTimeOffset> Start { get; }

        /// <summary>
        ///     When the activity ends.
        /// </summary>
        [JsonPropertyName("end")]
        public Optional<DateTimeOffset> End { get; }
    }
}
