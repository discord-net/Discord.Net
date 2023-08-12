using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class AutoModAction
    {
        [JsonProperty("type")]
        public AutoModActionType Type { get; set; }

        [JsonProperty("metadata")]
        public Optional<ActionMetadata> Metadata { get; set; }
    }
}
