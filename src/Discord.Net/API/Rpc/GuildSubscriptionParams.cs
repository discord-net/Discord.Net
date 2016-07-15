using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class GuildSubscriptionParams
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
