#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyVoiceChannelParams : ModifyGuildChannelParams
    {
        [ModelProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [ModelProperty("user_limit")]
        public Optional<int> UserLimit { get; set; }
    }
}
