using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.VoiceSocket
{
    public class IdentifyCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.Identify;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => true;

        [JsonProperty("server_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong GuildId { get; set; }
        [JsonProperty("user_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong UserId { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
