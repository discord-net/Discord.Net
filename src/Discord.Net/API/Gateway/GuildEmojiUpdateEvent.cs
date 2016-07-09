using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class GuildEmojiUpdateEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId;
        [JsonProperty("emojis")]
        public Emoji[] Emojis;
    }
}
