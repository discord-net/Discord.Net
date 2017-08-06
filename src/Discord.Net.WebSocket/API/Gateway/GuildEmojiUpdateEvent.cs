#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildEmojiUpdateEvent
    {
        [ModelProperty("guild_id")]
        public ulong GuildId { get; set; }
        [ModelProperty("emojis")]
        public Emoji[] Emojis { get; set; }
    }
}
