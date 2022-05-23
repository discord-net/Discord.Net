using Newtonsoft.Json;

namespace Discord.API
{
    internal class EmbedImage : IEmbedMediaModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public Optional<int> Height { get; set; }
        [JsonProperty("width")]
        public Optional<int> Width { get; set; }

        int? IEmbedMediaModel.Height { get => Height.ToNullable(); set => throw new System.NotSupportedException(); }
        int? IEmbedMediaModel.Width { get => Width.ToNullable(); set => throw new System.NotSupportedException(); }
    }
}
