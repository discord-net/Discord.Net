using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class MessageComponentInteractionData
    {
        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("component_type")]
        public ComponentType ComponentType { get; set; }
    }
}
