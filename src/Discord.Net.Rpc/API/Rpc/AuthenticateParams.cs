#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class AuthenticateParams
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
