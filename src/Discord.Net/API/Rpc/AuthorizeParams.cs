#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class AuthorizeParams
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("scopes")]
        public string[] Scopes { get; set; }
        [JsonProperty("rpc_token")]
        public Optional<string> RpcToken { get; set; }
    }
}
