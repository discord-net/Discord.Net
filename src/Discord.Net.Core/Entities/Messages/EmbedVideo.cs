using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A video featured in an <see cref="Embed"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedVideo
    {
        /// <summary>
        ///     Gets the URL of the video.
        /// </summary>
        /// <returns>
        ///     A string containing the URL of the image.
        /// </returns>
        public string Url { get; }
        /// <summary>
        ///     Gets the height of the video.
        /// </summary>
        /// <returns>
        ///     A <see cref="int"/> representing the height of this video if it can be retrieved; otherwise 
        ///     <c>null</c>.
        /// </returns>
        public int? Height { get; }
        /// <summary>
        ///     Gets the weight of the video.
        /// </summary>
        /// <returns>
        ///     A <see cref="int"/> representing the width of this video if it can be retrieved; otherwise 
        ///     <c>null</c>.
        /// </returns>
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
        /// <returns>
        ///     A string that resolves to <see cref="Url"/>.
        /// </returns>
        public override string ToString() => Url;
    }
}
