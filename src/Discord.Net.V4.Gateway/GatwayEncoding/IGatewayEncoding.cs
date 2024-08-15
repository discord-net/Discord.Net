using System;

namespace Discord.Gateway;

public interface IGatewayEncoding
{
    string Identifier { get; }

    TransportFormat Format { get; }
    
    ValueTask<T?> DecodeAsync<T>(Stream data, CancellationToken token = default);
    ValueTask EncodeAsync<T>(Stream stream, T value, CancellationToken token = default);
}
