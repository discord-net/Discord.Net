#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Invite
    {
        [ModelProperty("code")]
        public string Code { get; set; }
        [ModelProperty("guild")]
        public InviteGuild Guild { get; set; }
        [ModelProperty("channel")]
        public InviteChannel Channel { get; set; }
    }
}
