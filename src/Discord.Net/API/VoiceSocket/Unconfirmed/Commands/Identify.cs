using Newtonsoft.Json;

namespace Discord.API.VoiceSocket
{
    public class IdentifyCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.Identify;
        object IWebSocketMessage.Payload => this;

        [JsonProperty("server_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
