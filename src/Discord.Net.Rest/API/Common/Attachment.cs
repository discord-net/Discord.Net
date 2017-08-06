#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Attachment
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("filename")]
        public string Filename { get; set; }
        [ModelProperty("size")]
        public int Size { get; set; }
        [ModelProperty("url")]
        public string Url { get; set; }
        [ModelProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        [ModelProperty("height")]
        public Optional<int> Height { get; set; }
        [ModelProperty("width")]
        public Optional<int> Width { get; set; }
    }
}
