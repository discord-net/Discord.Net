#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Voice
{
    internal class VoiceSocketFrame
    {
        [ModelProperty("op")]
        public VoiceOpCode Operation { get; set; }
        [ModelProperty("t", ExcludeNull = true)]
        public string Type { get; set; }
        [ModelProperty("s", ExcludeNull = true)]
        public int? Sequence { get; set; }

        [ModelProperty("d")]
        [ModelSelector(nameof(Operation), ModelSelectorGroups.VoiceFrame)]
        [ModelSelector(nameof(Type), ModelSelectorGroups.VoiceDispatchFrame)]
        public object Payload { get; set; }
    }
}
