using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModifyTextChannelRequest : ModifyGuildChannelRequest
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }

        public ModifyTextChannelRequest(ulong channelId)
            : base(channelId)
        {
        }
    }
}
