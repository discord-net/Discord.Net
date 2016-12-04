using System.IO;

namespace Discord
{
    public struct Image
    {
        public Stream Stream { get; }
        public Image(Stream stream)
        {
            Stream = stream;
        }
        public Image(string path)
        {
            Stream = File.OpenRead(path);
        }
    }
}
