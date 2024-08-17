using System.IO.Compression;

namespace Discord.Gateway;

public sealed class ZLibCompression : IGatewayCompression
{
    public string Identifier => "zlib-stream";

    private readonly ZLibStream _decompressedStream;
    private readonly Stream _compressedStream;
    private readonly SemaphoreSlim _decompressionSemaphore;
    
    public ZLibCompression()
    {
        _decompressedStream = new(
            _compressedStream = DiscordGatewayClient.StreamManager.GetStream(nameof(ZLibCompression)),
            CompressionMode.Decompress,
            false
        );
        _decompressionSemaphore = new(1, 1);
    }
    
    public async ValueTask DecompressAsync(Stream source, Stream target, CancellationToken token = default)
    {
        await _decompressionSemaphore.WaitAsync(token);

        try
        {
            _compressedStream.Position = 0;
            await source.CopyToAsync(_compressedStream, token);
            
            _compressedStream.SetLength(_compressedStream.Position);
            _compressedStream.Position = 0;

            await _decompressedStream.CopyToAsync(target, token);
        }
        finally
        {
            _decompressionSemaphore.Release();
        }
    }

    public void Dispose()
    {
        _compressedStream.Dispose();
        _decompressedStream.Dispose();
    }
}
