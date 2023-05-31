using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents an outgoing file attachment used to send a file to discord.
    /// </summary>
    public struct FileAttachment : IDisposable
    {
        /// <summary>
        ///     Gets or sets the filename.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        ///     Gets or sets the description of the file.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets whether this file should be marked as a spoiler.
        /// </summary>
        public bool IsSpoiler { get; set; }

#pragma warning disable IDISP008
        /// <summary>
        ///     Gets the stream containing the file content.
        /// </summary>
        public Stream Stream { get; }
#pragma warning restore IDISP008 

        private bool _isDisposed;

        /// <summary>
        ///     Creates a file attachment from a stream.
        /// </summary>
        /// <param name="stream">The stream to create the attachment from.</param>
        /// <param name="fileName">The name of the attachment.</param>
        /// <param name="description">The description of the attachment.</param>
        /// <param name="isSpoiler">Whether or not the attachment is a spoiler.</param>
        public FileAttachment(Stream stream, string fileName, string description = null, bool isSpoiler = false)
        {
            _isDisposed = false;
            FileName = fileName;
            Description = description;
            Stream = stream;
            try
            {
                Stream.Position = 0;
            }
            catch { }
            IsSpoiler = isSpoiler;
        }

        /// <summary>
        ///     Create the file attachment from a file path.
        /// </summary>
        /// <remarks>
        ///     This file path is NOT validated and is passed directly into a
        ///     <see cref="File.OpenRead"/>.
        /// </remarks>
        /// <param name="path">The path to the file.</param>
        /// <param name="fileName">The name of the attachment.</param>
        /// <param name="description">The description of the attachment.</param>
        /// <param name="isSpoiler">Whether or not the attachment is a spoiler.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid
        /// characters as defined by <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
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
        public FileAttachment(string path, string fileName = null, string description = null, bool isSpoiler = false)
        {
            _isDisposed = false;
            Stream = File.OpenRead(path);
            FileName = fileName ?? Path.GetFileName(path);
            Description = description;
            IsSpoiler = isSpoiler;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                Stream?.Dispose();
                _isDisposed = true;
            }
        }
    }
}
