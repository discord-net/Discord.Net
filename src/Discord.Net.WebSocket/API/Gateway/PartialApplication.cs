using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Gateway
{
    internal class PartialApplication
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("flags")]
        public ApplicationFlags Flags { get; set; }
    }
}
