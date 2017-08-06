using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class GuildSummary
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
    }
}
