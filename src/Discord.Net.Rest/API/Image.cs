using System.IO;

namespace Discord.API
{
    internal struct Image
    {
        public Stream Stream { get; }
        public string Hash { get; }

        public Image(Stream stream)
        {
            Stream = stream;
            Hash = null;
        }
        public Image(string hash)
        {
            Stream = null;
            Hash = hash;
        }
    }
}
