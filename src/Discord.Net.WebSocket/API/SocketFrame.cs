#pragma warning disable CS1591
using Discord.Serialization;
using System;

namespace Discord.API
{
    internal class SocketFrame
    {
        [ModelProperty("op")]
        public int Operation { get; set; }
        [ModelProperty("t", IgnoreNull = true)]
        public string Type { get; set; }
        [ModelProperty("s", IgnoreNull = true)]
        public int? Sequence { get; set; }
        [ModelProperty("d")]
        public ReadOnlyBuffer<byte> Payload { get; set; }
    }
}
