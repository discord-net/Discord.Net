#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class GuildEmbed
    {
        [ModelProperty("enabled")]
        public bool Enabled { get; set; }
        [ModelProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
