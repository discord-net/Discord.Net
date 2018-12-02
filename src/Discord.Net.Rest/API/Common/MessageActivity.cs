using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    public class MessageActivity
    {
        [JsonProperty("type")]
        public Optional<MessageActivityType> Type { get; set; }
        [JsonProperty("party_id")]
        public Optional<string> PartyId { get; set; }
    }
}
