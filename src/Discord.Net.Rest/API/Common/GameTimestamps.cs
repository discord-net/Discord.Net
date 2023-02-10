using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class GameTimestamps
    {
        [JsonProperty("start")]
        [UnixTimestamp]
        public Optional<DateTimeOffset> Start { get; set; }
        [JsonProperty("end")]
        [UnixTimestamp]
        public Optional<DateTimeOffset> End { get; set; }
    }
}
