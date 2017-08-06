#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Voice
{
    internal class SessionDescriptionEvent
    {
        [ModelProperty("secret_key")]
        public byte[] SecretKey { get; set; }
        [ModelProperty("mode")]
        public string Mode { get; set; }
    }
}
