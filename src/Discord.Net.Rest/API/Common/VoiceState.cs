#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class VoiceState
    {
        [ModelProperty("guild_id")]
        public ulong? GuildId { get; set; }
        [ModelProperty("channel_id")]
        public ulong? ChannelId { get; set; }
        [ModelProperty("user_id")]
        public ulong UserId { get; set; }
        [ModelProperty("session_id")]
        public string SessionId { get; set; }
        [ModelProperty("deaf")]
        public bool Deaf { get; set; }
        [ModelProperty("mute")]
        public bool Mute { get; set; }
        [ModelProperty("self_deaf")]
        public bool SelfDeaf { get; set; }
        [ModelProperty("self_mute")]
        public bool SelfMute { get; set; }
        [ModelProperty("suppress")]
        public bool Suppress { get; set; }
    }
}
