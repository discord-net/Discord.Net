#pragma warning disable CS1591
using Discord.Serialization;
using System.Collections.Generic;

namespace Discord.API.Rpc
{
    internal class AuthorizeParams
    {
        [ModelProperty("client_id")]
        public string ClientId { get; set; }
        [ModelProperty("scopes")]
        public IReadOnlyCollection<string> Scopes { get; set; }
        [ModelProperty("rpc_token")]
        public Optional<string> RpcToken { get; set; }
    }
}
