using System.IO.Compression;

namespace Discord.Gateway;

public sealed class ZLibCompression : IGatewayCompression
{
    public static readonly ZLibCompression Instance = new();
    
    public string Identifier => "zlib-stream";

    public async ValueTask CompressAsync(Stream source, Stream target, CancellationToken token = default)
    {
        await using var compressionStream = new ZLibStream(target, CompressionMode.Compress, true);
        await source.CopyToAsync(compressionStream, token);
    }

    public async ValueTask DecompressAsync(Stream source, Stream target, CancellationToken token = default)
    {
        await using var compressionStream = new ZLibStream(source, CompressionMode.Decompress, true);
        await compressionStream.CopyToAsync(target, token);
    }
}
