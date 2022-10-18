using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildStickerUpdateEvent
    {
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("stickers")]
        public Sticker[] Stickers { get; set; }
    }
}
