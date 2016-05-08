using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class GuildPruneParams
    {
        [JsonProperty("days")]
        public int Days  = 30;
    }
}
