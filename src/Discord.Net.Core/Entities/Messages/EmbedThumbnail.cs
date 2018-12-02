using System.Diagnostics;

namespace Discord
{
    /// <summary> A thumbnail featured in an <see cref="Embed"/>. </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedThumbnail
    {
        /// <summary>
        ///     Gets the URL of the thumbnail.
        /// </summary>
        /// <returns>
        ///     A string containing the URL of the thumbnail.
        /// </returns>
        public string Url { get; }
        /// <summary>
        ///     Gets a proxied URL of this thumbnail.
        /// </summary>
        /// <returns>
        ///     A string containing the proxied URL of this thumbnail.
        /// </returns>
        public string ProxyUrl { get; }
        /// <summary>
        ///     Gets the height of this thumbnail.
        /// </summary>
        /// <returns>
        ///     A <see cref="int"/> representing the height of this thumbnail if it can be retrieved; otherwise 
        ///     <c>null</c>.
        /// </returns>
        public int? Height { get; }
        /// <summary>
        ///     Gets the width of this thumbnail.
        /// </summary>
        /// <returns>
        ///     A <see cref="int"/> representing the width of this thumbnail if it can be retrieved; otherwise 
        ///     <c>null</c>.
        /// </returns>
        public int? Width { get; }

        internal EmbedThumbnail(string url, string proxyUrl, int? height, int? width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }

        private string DebuggerDisplay => $"{Url} ({(Width != null && Height != null ? $"{Width}x{Height}" : "0x0")})";
        /// <summary>
        ///     Gets the URL of the thumbnail.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="Discord.EmbedThumbnail.Url" />.
        /// </returns>
        public override string ToString() => Url;
    }
}
