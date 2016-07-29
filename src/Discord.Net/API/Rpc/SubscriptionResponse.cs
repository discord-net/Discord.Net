using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class SubscriptionResponse
    {
        [JsonProperty("evt")]
        public string Event { get; set; }
    }
}
