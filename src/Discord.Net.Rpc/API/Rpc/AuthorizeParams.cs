#pragma warning disable CS1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Rpc
{
    internal class AuthorizeParams
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("scopes")]
        public IReadOnlyCollection<string> Scopes { get; set; }
        [JsonProperty("rpc_token")]
        public Optional<string> RpcToken { get; set; }
    }
}
