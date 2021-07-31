using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class StartThreadParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("duration")]
        public ThreadArchiveDuration Duration { get; set; }

        [JsonProperty("type")]
        public Optional<ThreadType> Type { get; set; }
    }
}
