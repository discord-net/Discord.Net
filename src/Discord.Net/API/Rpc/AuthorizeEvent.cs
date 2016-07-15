using Newtonsoft.Json;
using System;

namespace Discord.API.Rpc
{
    public class AuthorizeEvent
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
