#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class IntegrationAccount
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
    }
}
