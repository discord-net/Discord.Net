using System.IO;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Represents a multipart file to send in a request.
    /// </summary>
    public record MultipartFile
    {
        /// <summary>
        /// The file stream.
        /// </summary>
        public Stream? Stream { get; set; } // Required property candidate

        /// <summary>
        /// The file name.
        /// </summary>
        public string? Filename { get; set; } // Required property candidate
    }
}
