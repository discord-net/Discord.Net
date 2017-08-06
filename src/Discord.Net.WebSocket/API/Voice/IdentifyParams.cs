#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Voice
{
    internal class IdentifyParams
    {
        [ModelProperty("server_id")]
        public ulong GuildId { get; set; }
        [ModelProperty("user_id")]
        public ulong UserId { get; set; }
        [ModelProperty("session_id")]
        public string SessionId { get; set; }
        [ModelProperty("token")]
        public string Token { get; set; }
    }
}
