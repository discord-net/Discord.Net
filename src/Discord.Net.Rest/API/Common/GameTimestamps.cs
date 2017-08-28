using System;
using Newtonsoft.Json;

namespace Discord.API
{
    internal class GameTimestamps
    {
        [JsonProperty("start")]
        public DateTimeOffset Start { get; set; }
        [JsonProperty("end")]
        public DateTimeOffset End { get; set; }
    }
}