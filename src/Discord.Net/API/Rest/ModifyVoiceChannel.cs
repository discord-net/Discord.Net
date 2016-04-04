using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModifyVoiceChannelRequest : ModifyGuildChannelRequest
    {
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }

        public ModifyVoiceChannelRequest(ulong channelId)
            : base(channelId)
        {
        }
    }
}
