using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an attachment object.
    /// </summary>
    public record Attachment
    {
        /// <summary>
        ///     Creates a <see cref="Attachment"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Attachment id.</param>
        /// <param name="filename">Name of file attached.</param>
        /// <param name="contentType">The attachment's media type.</param>
        /// <param name="size">Size of file in bytes.</param>
        /// <param name="url">Source url of file.</param>
        /// <param name="proxyUrl">A proxied url of file.</param>
        /// <param name="height">Height of file (if image).</param>
        /// <param name="width">Width of file (if image).</param>
        [JsonConstructor]
        public Attachment(Snowflake id, string filename, Optional<string> contentType, int size, string url, string proxyUrl, Optional<int?> height, Optional<int?> width)
        {
            Id = id;
            Filename = filename;
            ContentType = contentType;
            Size = size;
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }

        /// <summary>
        ///     Attachment id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     Name of file attached.
        /// </summary>
        [JsonPropertyName("filename")]
        public string Filename { get; }

        /// <summary>
        ///     The attachment's media type.
        /// </summary>
        [JsonPropertyName("content_type")]
        public Optional<string> ContentType { get; }

        /// <summary>
        ///     Size of file in bytes.
        /// </summary>
        [JsonPropertyName("size")]
        public int Size { get; }

        /// <summary>
        ///     Source url of file.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }

        /// <summary>
        ///     A proxied url of file.
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; }

        /// <summary>
        ///     Height of file (if image).
        /// </summary>
        [JsonPropertyName("height")]
        public Optional<int?> Height { get; }

        /// <summary>
        ///     Width of file (if image).
        /// </summary>
        [JsonPropertyName("width")]
        public Optional<int?> Width { get; }
    }
}
