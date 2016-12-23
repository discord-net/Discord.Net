using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Discord.API
{
    public class Reaction
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("me")]
        public bool Me { get; set; }
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }
    }
}
