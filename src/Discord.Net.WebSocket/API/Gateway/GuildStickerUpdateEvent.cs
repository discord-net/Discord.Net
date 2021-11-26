using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class GuildStickerUpdateEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("stickers")]
        public Sticker[] Stickers { get; set; }
    }
}
