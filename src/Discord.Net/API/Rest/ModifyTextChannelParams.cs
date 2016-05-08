using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyTextChannelParams : ModifyGuildChannelParams
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }
    }
}
