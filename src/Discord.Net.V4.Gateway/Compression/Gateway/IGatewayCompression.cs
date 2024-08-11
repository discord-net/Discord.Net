namespace Discord.Gateway;

public interface IGatewayCompression
{
    string Identifier { get; }

    ValueTask CompressAsync(Stream source, Stream target, CancellationToken token = default);
    ValueTask DecompressAsync(Stream stream, Stream target, CancellationToken token = default);

    static IGatewayCompression ZLib => ZLibCompression.Instance;
}
