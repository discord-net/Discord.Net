#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Application
    {
        [ModelProperty("description")]
        public string Description { get; set; }
        [ModelProperty("rpc_origins")]
        public string[] RPCOrigins { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("icon")]
        public string Icon { get; set; }

        [ModelProperty("flags"), Int53]
        public Optional<ulong> Flags { get; set; }
        [ModelProperty("owner")]
        public Optional<User> Owner { get; set; }
    }
}
