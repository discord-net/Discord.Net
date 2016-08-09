#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class ErrorEvent
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
