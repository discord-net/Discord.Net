#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class GuildMember
    {
        [ModelProperty("user")]
        public User User { get; set; }
        [ModelProperty("status")]
        public UserStatus Status { get; set; }
        /*[ModelProperty("activity")]
        public object Activity { get; set; }*/
    }
}
