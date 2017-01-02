#pragma warning disable CS1591
using Newtonsoft.Json;
using System;

namespace Discord.API.Rpc
{
    internal class AuthenticateResponse
    {
        [JsonProperty("application")]
        public Application Application { get; set; }
        [JsonProperty("expires")]
        public DateTimeOffset Expires { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("scopes")]
        public string[] Scopes { get; set; }
    }
}
