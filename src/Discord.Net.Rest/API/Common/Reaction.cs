using Newtonsoft.Json;

namespace Discord.API
{
    internal class Reaction : IReactionMetadataModel
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("me")]
        public bool Me { get; set; }
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        int IReactionMetadataModel.Count { get => Count; set => throw new System.NotSupportedException(); }
        bool IReactionMetadataModel.Me { get => Me; set => throw new System.NotSupportedException(); }
        IEmojiModel IReactionMetadataModel.Emoji { get => Emoji; set => throw new System.NotSupportedException(); }
    }
}
