#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class VoiceStateUpdateParams
    {
        [ModelProperty("self_mute")]
        public bool SelfMute { get; set; }
        [ModelProperty("self_deaf")]
        public bool SelfDeaf { get; set; }

        [ModelProperty("guild_id")]
        public ulong? GuildId { get; set; }
        [ModelProperty("channel_id")]
        public ulong? ChannelId { get; set; }
    }
}
