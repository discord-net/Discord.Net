using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedImage
    {
        public string Url { get; }
        public string ProxyUrl { get; }
        public int? Height { get; }
        public int? Width { get; }

        internal EmbedImage(string url, string proxyUrl, int? height, int? width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }

        private string DebuggerDisplay => $"{Url} ({(Width != null && Height != null ? $"{Width}x{Height}" : "0x0")})";
        public override string ToString() => Url;
    }
}
