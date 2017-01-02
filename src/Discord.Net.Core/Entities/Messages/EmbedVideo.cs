using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedVideo
    {
        public string Url { get; }
        public int? Height { get; }
        public int? Width { get; }

        internal EmbedVideo(string url, int? height, int? width)
        {
            Url = url;
            Height = height;
            Width = width;
        }

        private string DebuggerDisplay => $"{Url} ({(Width != null && Height != null ? $"{Width}x{Height}" : "0x0")})";
        public override string ToString() => Url;
    }
}
