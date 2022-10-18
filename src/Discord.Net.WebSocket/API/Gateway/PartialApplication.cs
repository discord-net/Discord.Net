using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Gateway
{
    internal class PartialApplication
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("flags")]
        public ApplicationFlags Flags { get; set; }
    }
}
