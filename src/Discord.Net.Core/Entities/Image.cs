using System.IO;
namespace Discord
{
    /// <summary>
    ///     An image that will be uploaded to Discord.
    /// </summary>
    public struct Image
    {
        /// <summary>
        ///     Gets the stream to be uploaded to Discord.
        /// </summary>
        public Stream Stream { get; }
        /// <summary>
        ///     Create the image with a <see cref="System.IO.Stream" /> .
        /// </summary>
        /// <param name="stream">
        ///     The <see cref="System.IO.Stream" /> to create the image with. Note that this must be some type of stream
        ///     with the contents of a file in it.
        /// </param>
        public Image(Stream stream)
        {
            Stream = stream;
        }
#if FILESYSTEM
        /// <summary>
        ///     Create the image from a file path.
        /// </summary>
        /// <remarks>
        ///     This file <paramref name="path" /> is NOT validated, and is passed directly into a
        ///     <see cref="File.OpenRead" />
        /// </remarks>
        /// <param name="path">The path to the file.</param>
        public Image(string path)
        {
            Stream = File.OpenRead(path);
        }
#endif
    }
}
