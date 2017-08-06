#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyGuildMemberParams
    {
        [ModelProperty("mute")]
        public Optional<bool> Mute { get; set; }
        [ModelProperty("deaf")]
        public Optional<bool> Deaf { get; set; }
        [ModelProperty("nick")]
        public Optional<string> Nickname { get; set; }
        [ModelProperty("roles")]
        public Optional<ulong[]> RoleIds { get; set; }
        [ModelProperty("channel_id")]
        public Optional<ulong> ChannelId { get; set; }
    }
}
