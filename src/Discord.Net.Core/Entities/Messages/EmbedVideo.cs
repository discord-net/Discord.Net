using System;
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

        public static bool operator ==(EmbedVideo? left, EmbedVideo? right)
            => left is null ? right is null
                : left.Equals(right);

        public static bool operator !=(EmbedVideo? left, EmbedVideo? right)
            => !(left == right);

        public override bool Equals(object obj)
            => obj is not null && GetType() == obj.GetType() && Equals(obj as EmbedVideo?);

        public bool Equals(EmbedVideo embedVideo)
            => GetHashCode() == embedVideo.GetHashCode();

        public override int GetHashCode()
            => new { Width, Height, Url }.GetHashCode();
    }
}
