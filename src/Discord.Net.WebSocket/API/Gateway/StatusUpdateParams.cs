#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class StatusUpdateParams
    {
        [ModelProperty("status")]
        public UserStatus Status { get; set; }
        [ModelProperty("since"), Int53]
        public long? IdleSince { get; set; }
        [ModelProperty("afk")]
        public bool IsAFK { get; set; }
        [ModelProperty("game")]
        public Game Game { get; set; }
    }
}
