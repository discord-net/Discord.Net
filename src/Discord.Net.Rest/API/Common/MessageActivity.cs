using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    public class MessageActivity
    {
        [JsonPropertyName("type")]
        public Optional<MessageActivityType> Type { get; set; }
        [JsonPropertyName("party_id")]
        public Optional<string> PartyId { get; set; }
    }
}
