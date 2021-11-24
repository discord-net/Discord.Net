using Newtonsoft.Json;

namespace Discord.API
{
    internal class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
