using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class RpcMessage
    {
        [JsonProperty("cmd")]
        public string Cmd { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("evt")]
        public string Event { get; set; }
        [JsonProperty("data")]
        public object Data { get; set; }
        [JsonProperty("args")]
        public object Args { get; set; }
    }
}
