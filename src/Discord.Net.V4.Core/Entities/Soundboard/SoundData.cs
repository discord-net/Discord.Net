using System.Security.Cryptography;
using System.Text;

namespace Discord;

public readonly struct SoundData(SoundData.SoundKind kind, Stream stream)
{
    public enum SoundKind
    {
        Mp3,
        Ogg
    }
    
    public SoundKind Kind { get; } = kind;
    public Stream Stream { get; } = stream;

    public string ToSoundData()
    {
        var sb = new StringBuilder($"data:audio/{Kind.ToString().ToLowerInvariant()};base64,");
        using (var base64Stream = new CryptoStream(Stream, new ToBase64Transform(), CryptoStreamMode.Read))
        using (var reader = new StreamReader(base64Stream))
        {
            sb.Append(reader.ReadToEnd());
        }

        return sb.ToString();
    }
}