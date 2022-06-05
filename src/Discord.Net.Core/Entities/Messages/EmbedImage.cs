using System;
using System.Diagnostics;

namespace Discord
{
    /// <summary> An image for an <see cref="Embed"/>. </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedImage
    {
        /// <summary>
        ///     Gets the URL of the image.
        /// </summary>
        /// <returns>
        ///     A string containing the URL of the image.
        /// </returns>
        public string Url { get; }
        /// <summary>
        ///     Gets a proxied URL of this image.
        /// </summary>
        /// <returns>
        ///     A string containing the proxied URL of this image.
        /// </returns>
        public string ProxyUrl { get; }
        /// <summary>
        ///     Gets the height of this image.
        /// </summary>
        /// <returns>
        ///     A <see cref="int"/> representing the height of this image if it can be retrieved; otherwise 
        ///     <c>null</c>.
        /// </returns>
        public int? Height { get; }
        /// <summary>
        ///     Gets the width of this image.
        /// </summary>
        /// <returns>
        ///     A <see cref="int"/> representing the width of this image if it can be retrieved; otherwise 
        ///     <c>null</c>.
        /// </returns>
        public int? Width { get; }

        internal EmbedImage(string url, string proxyUrl, int? height, int? width)
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
        ///     A string that resolves to <see cref="Discord.EmbedImage.Url"/> .
        /// </returns>
        public override string ToString() => Url;

        public static bool operator ==(EmbedImage? left, EmbedImage? right)
            => left is null ? right is null
                : left.Equals(right);

        public static bool operator !=(EmbedImage? left, EmbedImage? right)
            => !(left == right);

        public override bool Equals(object obj)
            => obj is not null && GetType() == obj.GetType() && Equals(obj as EmbedImage?);

        public bool Equals(EmbedImage embedImage)
            => GetHashCode() == embedImage.GetHashCode();

        public override int GetHashCode()
            => (Height, Width, Url, ProxyUrl).GetHashCode();
    }
}
