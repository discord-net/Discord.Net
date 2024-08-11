using System.IO.Compression;

namespace Discord.Gateway;

public sealed class ZLibCompression : IGatewayCompression
{
    public static readonly IGatewayCompression Instance = new ZLibCompression();

    public string Identifier => "zlib-stream";

    public async ValueTask CompressAsync(Stream source, Stream target, CancellationToken token = default)
    {
        await using var compressionStream = new ZLibStream(source, CompressionMode.Compress, true);
        await compressionStream.CopyToAsync(target, token);
    }

    public async ValueTask DecompressAsync(Stream source, Stream target, CancellationToken token = default)
    {
        await using var compressionStream = new ZLibStream(source, CompressionMode.Decompress, true);
        await compressionStream.CopyToAsync(target, token);
    }
}
