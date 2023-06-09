using System.IO;

namespace Discord.API;

internal class Sound
{
    public Stream Stream { get; }
    public string Hash { get; }

    public Sound(Stream stream)
    {
        Stream = stream;
        Hash = null;
    }
    public Sound(string hash)
    {
        Stream = null;
        Hash = hash;
    }
}
