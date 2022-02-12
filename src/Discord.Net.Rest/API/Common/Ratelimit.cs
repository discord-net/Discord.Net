using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class Ratelimit
    {
        [JsonProperty("global")]
        public bool Global { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("retry_after")]
        public double RetryAfter { get; set; }
    }
}
