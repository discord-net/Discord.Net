using System;
using System.IO;

namespace Discord
{
    /// <summary>
    ///     An sound that will be uploaded to Discord.
    /// </summary>
    public struct Sound : IDisposable
    {
        private bool _isDisposed;

        /// <summary>
        ///     Gets the stream to be uploaded to Discord.
        /// </summary>
#pragma warning disable IDISP008
        public Stream Stream { get; }
#pragma warning restore IDISP008
        /// <summary>
        ///     Create the sound with a <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">
        ///     The <see cref="System.IO.Stream" /> to create the sound with. Note that this must be some type of stream
        ///     with the contents of a file in it.
        /// </param>
        public Sound(Stream stream)
        {
            _isDisposed = false;
            Stream = stream;
        }

        /// <summary>
        ///     Create the sound from a file path.
        /// </summary>
        /// <remarks>
        ///     This file path is NOT validated and is passed directly into a
        ///     <see cref="File.OpenRead"/>.
        /// </remarks>
        /// <param name="path">The path to the file.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid
        /// characters as defined by <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length. For example, on
        /// Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260
        /// characters.
        /// </exception>
        /// <exception cref="System.NotSupportedException"><paramref name="path" /> is in an invalid format.</exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified <paramref name="path"/> is invalid, (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path" /> was not found.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file. </exception>
        public Sound(string path)
        {
            _isDisposed = false;
            Stream = File.OpenRead(path);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_isDisposed)
            {
#pragma warning disable IDISP007
                Stream?.Dispose();
#pragma warning restore IDISP007

                _isDisposed = true;
            }
        }
    }
}
