using Newtonsoft.Json;

namespace Discord.API.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateVoiceCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.VoiceStateUpdate;
        object IWebSocketMessage.Payload => this;

        [JsonProperty("guild_id")]
        public ulong? GuildId { get; set; }
        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }
        [JsonProperty("self_mute")]
        public bool IsSelfMuted { get; set; }
        [JsonProperty("self_deaf")]
        public bool IsSelfDeafened { get; set; }
    }
}
