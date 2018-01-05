#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class SubscriptionResponse
    {
        [JsonProperty("evt")]
        public string Event { get; set; }
    }
}
