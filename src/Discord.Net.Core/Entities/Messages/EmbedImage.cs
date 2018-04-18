using System.Diagnostics;

namespace Discord
{
    /// <summary> An image for an <see cref="Embed"/>. </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedImage
    {
        /// <summary> Gets the URL of the image.</summary>
        public string Url { get; }
        /// <summary> Gets the proxified URL of the image.</summary>
        public string ProxyUrl { get; }
        /// <summary> Gets the height of the image if any is set. </summary>
        public int? Height { get; }
        /// <summary> Gets the width of the image if any is set. </summary>
        public int? Width { get; }

        internal EmbedImage(string url, string proxyUrl, int? height, int? width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }

        private string DebuggerDisplay => $"{Url} ({(Width != null && Height != null ? $"{Width}x{Height}" : "0x0")})";
        public override string ToString() => Url.ToString();
    }
}
