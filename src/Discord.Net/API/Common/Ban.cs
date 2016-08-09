#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    public class Ban
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
