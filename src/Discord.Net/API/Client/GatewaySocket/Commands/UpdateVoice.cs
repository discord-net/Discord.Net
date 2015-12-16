using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UpdateVoiceCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.StatusUpdate;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => false;

        [JsonProperty("guild_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong GuildId { get; set; }
        [JsonProperty("channel_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong ChannelId { get; set; }
        [JsonProperty("self_mute")]
        public bool IsSelfMuted { get; set; }
        [JsonProperty("self_deaf")]
        public bool IsSelfDeafened { get; set; }
    }
}
