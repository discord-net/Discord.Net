using System.IO;

namespace Discord.API
{
    internal struct Image
    {
        public Stream Stream { get; }
        public ImageFormat StreamFormat { get; }
        public string Hash { get; }

        public Image(Stream stream, ImageFormat format)
        {
            Stream = stream;
            StreamFormat = format;
            Hash = null;
        }
        public Image(string hash)
        {
            Stream = null;
            StreamFormat = ImageFormat.Jpeg;
            Hash = hash;
        }
    }
}
