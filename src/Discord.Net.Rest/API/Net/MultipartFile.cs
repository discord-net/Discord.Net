using System.IO;

namespace Discord.Net.Rest
{
    internal struct MultipartFile
    {
        public Stream Stream { get; }
        public string Filename { get; }
        public string ContentType { get; }

        public MultipartFile(Stream stream, string filename, string contentType = null)
        {
            Stream = stream;
            Filename = filename;
            ContentType = contentType;
        }
    }
}
