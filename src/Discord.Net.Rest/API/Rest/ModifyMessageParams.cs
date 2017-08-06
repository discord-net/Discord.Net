#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyMessageParams
    {
        [ModelProperty("content")]
        public Optional<string> Content { get; set; }
        [ModelProperty("embed")]
        public Optional<Embed> Embed { get; set; }
    }
}
