#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class AuthorizeResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
