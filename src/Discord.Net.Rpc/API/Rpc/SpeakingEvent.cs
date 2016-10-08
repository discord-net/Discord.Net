#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class SpeakingEvent
    {
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
    }
}
