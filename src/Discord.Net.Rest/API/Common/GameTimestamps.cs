using System;
using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class GameTimestamps
    {
        [JsonPropertyName("start")]
        [UnixTimestamp]
        public Optional<DateTimeOffset> Start { get; set; }
        [JsonPropertyName("end")]
        [UnixTimestamp]
        public Optional<DateTimeOffset> End { get; set; }
    }
}
