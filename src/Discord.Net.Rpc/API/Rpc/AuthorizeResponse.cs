#pragma warning disable CS1591
using Newtonsoft.Json;
using System;

namespace Discord.API.Rpc
{
    public class AuthorizeResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
