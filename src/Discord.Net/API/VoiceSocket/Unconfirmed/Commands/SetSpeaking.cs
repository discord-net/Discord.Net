using Newtonsoft.Json;

namespace Discord.API.VoiceSocket
{
    public class SetSpeakingCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.Speaking;
        object IWebSocketMessage.Payload => this;

        [JsonProperty("speaking")]
        public bool IsSpeaking { get; set; }
        [JsonProperty("delay")]
        public int Delay { get; set; }
    }
}
