using System.IO;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Container to send a base64 encoded image to Discord.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/reference#image-data"/>
    /// </remarks>
    public struct Image
    {
        /// <summary>
        /// The image stream.
        /// </summary>
        public Stream? Stream { get; }

        /// <summary>
        /// The format of the image.
        /// </summary>
        public ImageFormat StreamFormat { get; }

        /// <summary>
        /// The image hash.
        /// </summary>
        public string? Hash { get; }

        /// <summary>
        /// Creates an <see cref="Image"/> based on a <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// Image stream.
        /// </param>
        /// <param name="format">
        /// Image format.
        /// </param>
        public Image(Stream stream, ImageFormat format)
        {
            Stream = stream;
            StreamFormat = format;
            Hash = null;
        }

        /// <summary>
        /// Creates an <see cref="Image"/> based on the image hash.
        /// </summary>
        /// <param name="hash">
        /// Image hash.
        /// </param>
        public Image(string hash)
        {
            Stream = null;
            StreamFormat = ImageFormat.Jpeg;
            Hash = hash;
        }
    }
}
