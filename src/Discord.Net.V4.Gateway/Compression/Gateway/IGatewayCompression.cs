namespace Discord.Gateway;

public interface IGatewayCompression : IDisposable
{
    string Identifier { get; }

    ValueTask DecompressAsync(Stream stream, Stream target, CancellationToken token = default);
}
