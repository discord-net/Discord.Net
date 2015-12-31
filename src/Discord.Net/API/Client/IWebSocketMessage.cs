using Newtonsoft.Json;

namespace Discord.API.Client
{
    public interface IWebSocketMessage
    {
        int OpCode { get; }
        object Payload { get; }
        bool IsPrivate { get; }
    }
    public class WebSocketMessage
    {
        [JsonProperty("op")]
        public int? Operation { get; set; }
        [JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
        public uint? Sequence { get; set; }
        [JsonProperty("d")]
        public object Payload { get; set; }

        public WebSocketMessage() { }
        public WebSocketMessage(IWebSocketMessage msg)
        {
            Operation = msg.OpCode;
            Payload = msg.Payload;
        }
    }
}
