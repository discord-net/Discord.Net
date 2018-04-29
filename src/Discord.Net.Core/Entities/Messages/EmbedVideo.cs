using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A video featured in an <see cref="Embed" />.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedVideo
    {
        /// <summary>
        ///     Gets the URL of the video.
        /// </summary>
        public string Url { get; }
        /// <summary>
        ///     Gets the height of the video, or <see langword="null"/> if none.
        /// </summary>
        public int? Height { get; }
        /// <summary>
        ///     Gets the weight of the video, or <see langword="null"/> if none.
        /// </summary>
        public int? Width { get; }

        internal EmbedVideo(string url, int? height, int? width)
        {
            Url = url;
            Height = height;
            Width = width;
        }

        private string DebuggerDisplay => $"{Url} ({(Width != null && Height != null ? $"{Width}x{Height}" : "0x0")})";
        /// <summary>
        ///     Gets the URL of the video.
        /// </summary>
        public override string ToString() => Url;
    }
}
