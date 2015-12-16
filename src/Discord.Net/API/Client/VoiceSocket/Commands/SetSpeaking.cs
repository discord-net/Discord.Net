using Newtonsoft.Json;

namespace Discord.API.Client.VoiceSocket
{
    public sealed class SetSpeakingCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.Speaking;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => false;

        [JsonProperty("speaking")]
        public bool IsSpeaking { get; set; }
        [JsonProperty("delay")]
        public int Delay { get; set; }
    }
}
