using System;
namespace Discord.Gateway
{
    public interface IGatewayEncoding
    {
        T Decode<T>(Stream data);
        ReadOnlyMemory<byte> Encode<T>(T value);

        T? ToObject<T>(object? obj);
    }
}

