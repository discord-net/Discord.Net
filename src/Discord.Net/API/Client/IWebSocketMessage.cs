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
        public int Operation { get; }
        [JsonProperty("d")]
        public object Payload { get; }
        [JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; }
        [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
        public int? Sequence { get; }

        public WebSocketMessage() { }
        public WebSocketMessage(IWebSocketMessage msg)
        {
            Operation = msg.OpCode;
            Payload = msg.Payload;
        }
    }
}
