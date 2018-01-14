#pragma warning disable CS1591
using Newtonsoft.Json;
using System;

namespace Discord.API.Rpc
{
    internal class RpcFrame
    {
        [JsonProperty("cmd")]
        public string Cmd { get; set; }
        [JsonProperty("nonce")]
        public Optional<Guid?> Nonce { get; set; }
        [JsonProperty("evt")]
        public Optional<string> Event { get; set; }
        [JsonProperty("data")]
        public Optional<object> Data { get; set; }
        [JsonProperty("args")]
        public object Args { get; set; }
    }
}
