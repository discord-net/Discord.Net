using Newtonsoft.Json;

namespace Discord.API
{
    internal class Attachment : IAttachmentModel
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("description")]
        public Optional<string> Description { get; set; }
        [JsonProperty("content_type")]
        public Optional<string> ContentType { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public Optional<int> Height { get; set; }
        [JsonProperty("width")]
        public Optional<int> Width { get; set; }
        [JsonProperty("ephemeral")]
        public Optional<bool> Ephemeral { get; set; }

        string IAttachmentModel.FileName { get => Filename; set => throw new System.NotSupportedException(); }
        string IAttachmentModel.Description { get => Description.GetValueOrDefault(); set => throw new System.NotSupportedException(); }
        string IAttachmentModel.ContentType { get => ContentType.GetValueOrDefault(); set => throw new System.NotSupportedException(); }
        int IAttachmentModel.Size { get => Size; set => throw new System.NotSupportedException(); }
        string IAttachmentModel.Url { get => Url; set => throw new System.NotSupportedException(); }
        string IAttachmentModel.ProxyUrl { get => ProxyUrl; set => throw new System.NotSupportedException(); }
        int? IAttachmentModel.Height { get => Height.ToNullable(); set => throw new System.NotSupportedException(); }
        int? IAttachmentModel.Width { get => Width.ToNullable(); set => throw new System.NotSupportedException(); }
        bool IAttachmentModel.Ephemeral { get => Ephemeral.GetValueOrDefault(); set => throw new System.NotSupportedException(); }
        ulong IEntityModel<ulong>.Id { get => Id; set => throw new System.NotSupportedException(); }
    }
}
