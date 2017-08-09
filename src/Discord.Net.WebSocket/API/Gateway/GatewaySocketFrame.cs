#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class GatewaySocketFrame
    {
        [ModelProperty("op")]
        public GatewayOpCode Operation { get; set; }
        [ModelProperty("t", ExcludeNull = true)]
        public string Type { get; set; }
        [ModelProperty("s", ExcludeNull = true)]
        public int? Sequence { get; set; }

        [ModelProperty("d")]
        [ModelSelector(nameof(Operation), ModelSelectorGroups.GatewayFrame)]
        [ModelSelector(nameof(Type), ModelSelectorGroups.GatewayDispatchFrame)]
        public object Payload { get; set; }
    }
}
