using Newtonsoft.Json;

namespace Discord.API
{
    internal class MessageActivity
    {
        [JsonProperty("type")]
        public Optional<MessageActivityType> Type { get; set; }
        [JsonProperty("party_id")]
        public Optional<string> PartyId { get; set; }
    }
}
