using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyTextChannelParams : ModifyGuildChannelParams
    {
        [JsonProperty("topic")]
        internal Optional<string> _topic;
        public string Topic { set { _topic = value; } }
    }
}
