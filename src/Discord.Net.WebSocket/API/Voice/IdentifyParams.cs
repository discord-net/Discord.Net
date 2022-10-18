using System.Text.Json.Serialization;

namespace Discord.API.Voice
{
    internal class IdentifyParams
    {
        [JsonPropertyName("server_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
