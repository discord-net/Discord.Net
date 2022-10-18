using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class MessageInteraction
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("type")]
        public InteractionType Type { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("user")]
        public User User { get; set; }
    }
}
