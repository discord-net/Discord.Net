#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Ban
    {
        [ModelProperty("user")]
        public User User { get; set; }
        [ModelProperty("reason")]
        public string Reason { get; set; }
    }
}
