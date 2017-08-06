#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyGuildEmbedParams
    {        
        [ModelProperty("enabled")]
        public Optional<bool> Enabled { get; set; }
        [ModelProperty("channel")]
        public Optional<ulong?> ChannelId { get; set; }
    }
}
