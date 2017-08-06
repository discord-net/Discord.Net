#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class ResumeParams
    {
        [ModelProperty("token")]
        public string Token { get; set; }
        [ModelProperty("session_id")]
        public string SessionId { get; set; }
        [ModelProperty("seq")]
        public int Sequence { get; set; }
    }
}
