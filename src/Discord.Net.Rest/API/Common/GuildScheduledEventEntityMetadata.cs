using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class GuildScheduledEventEntityMetadata
    {
        [JsonPropertyName("location")]
        public Optional<string> Location { get; set; }
    }
}
