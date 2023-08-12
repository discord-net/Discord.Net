using System;
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
        ///     <see langword="null" />.
        /// </returns>
        public int? Height { get; }
        /// <summary>
        ///     Gets the width of this thumbnail.
        /// </summary>
        /// <returns>
        ///     A <see cref="int"/> representing the width of this thumbnail if it can be retrieved; otherwise 
        ///     <see langword="null" />.
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

        public static bool operator ==(EmbedThumbnail? left, EmbedThumbnail? right)
            => left is null ? right is null
                : left.Equals(right);

        public static bool operator !=(EmbedThumbnail? left, EmbedThumbnail? right)
            => !(left == right);

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="EmbedThumbnail"/>.
        /// </summary>
        /// <remarks>
        /// If the object passes is an <see cref="EmbedThumbnail"/>, <see cref="Equals(EmbedThumbnail?)"/> will be called to compare the 2 instances
        /// </remarks>
        /// <param name="obj">The object to compare with the current <see cref="EmbedThumbnail"/></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is EmbedThumbnail embedThumbnail && Equals(embedThumbnail);

        /// <summary>
        /// Determines whether the specified <see cref="EmbedThumbnail"/> is equal to the current <see cref="EmbedThumbnail"/>
        /// </summary>
        /// <param name="embedThumbnail">The <see cref="EmbedThumbnail"/> to compare with the current <see cref="EmbedThumbnail"/></param>
        /// <returns></returns>
        public bool Equals(EmbedThumbnail? embedThumbnail)
            => GetHashCode() == embedThumbnail?.GetHashCode();

        /// <inheritdoc />
        public override int GetHashCode()
            => (Width, Height, Url, ProxyUrl).GetHashCode();
    }
}
