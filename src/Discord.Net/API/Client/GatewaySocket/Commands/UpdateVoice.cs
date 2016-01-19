using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateVoiceCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.VoiceStateUpdate;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => false;

        [JsonProperty("guild_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? GuildId { get; set; }
        [JsonProperty("channel_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? ChannelId { get; set; }
        [JsonProperty("self_mute")]
        public bool IsSelfMuted { get; set; }
        [JsonProperty("self_deaf")]
        public bool IsSelfDeafened { get; set; }
    }
}
