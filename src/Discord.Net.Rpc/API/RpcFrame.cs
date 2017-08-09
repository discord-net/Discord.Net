#pragma warning disable CS1591
using Discord.Serialization;
using System;

namespace Discord.API.Rpc
{
    internal class RpcFrame
    {
        [ModelProperty("cmd")]
        public string Cmd { get; set; }
        [ModelProperty("nonce")]
        public Optional<Guid?> Nonce { get; set; }
        [ModelProperty("evt")]
        public Optional<string> Event { get; set; }
        [ModelProperty("data")]
        public Optional<ReadOnlyBuffer<byte>> Data { get; set; }

        [ModelProperty("args")]
        [ModelSelector(ModelSelectorGroups.RpcFrame, nameof(Event))]
        public object Args { get; set; }
    }
}
