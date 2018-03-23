using System;
using System.Diagnostics;

namespace Discord
{
    /// <summary> A thumbnail featured in an <see cref="Embed"/>. </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedThumbnail
    {
        /// <summary> Gets the URL of the thumbnail.</summary>
        public string Url { get; }
        /// <summary> Gets the proxified URL of the thumbnail.</summary>
        public string ProxyUrl { get; }
        /// <summary> Gets the height of the thumbnail if any is set. </summary>
        public int? Height { get; }
        /// <summary> Gets the width of the thumbnail if any is set. </summary>
        public int? Width { get; }

        internal EmbedThumbnail(string url, string proxyUrl, int? height, int? width)
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
