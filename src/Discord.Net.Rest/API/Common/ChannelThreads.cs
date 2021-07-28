using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class ChannelThreads
    {
        [JsonProperty("threads")]
        public Channel[] Threads { get; set; }

        [JsonProperty("members")]
        public ThreadMember[] Members { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
    }
}
