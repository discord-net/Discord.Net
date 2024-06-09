using System.Security.Cryptography;
using System.Text;

namespace Discord;

public readonly struct Image(Stream stream)
{
    public Stream Stream { get; } = stream;

    public string ToImageData()
    {
        var sb = new StringBuilder("data:image/jpeg;base64,");
        using (var base64Stream = new CryptoStream(Stream, new ToBase64Transform(), CryptoStreamMode.Read))
        using (var reader = new StreamReader(base64Stream))
        {
            sb.Append(reader.ReadToEnd());
        }

        return sb.ToString();
    }
}
