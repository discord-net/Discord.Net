#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class ReadyEvent
    {
        [ModelProperty("v")]
        public int Version { get; set; }
        [ModelProperty("config")]
        public RpcConfig Config { get; set; }
    }
}
