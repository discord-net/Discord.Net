using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class Ratelimit
    {
        [JsonPropertyName("global")]
        public bool Global { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("retry_after")]
        public double RetryAfter { get; set; }
    }
}
