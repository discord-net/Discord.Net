using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Gateway
{
    public sealed class JsonEncoding : IGatewayEncoding
    {
        public T? Decode<T>(Stream data)
            => JsonSerializer.Deserialize<T>(data);

        public ReadOnlyMemory<byte> Encode<T>(T value)
            => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));

        public T? ToObject<T>(object? obj)
        {
            if (obj is null)
                return default;

            if (obj is JsonDocument jdoc)
                return jdoc.Deserialize<T>();

            throw new ArgumentException($"Unknown object type {obj.GetType().Name}", nameof(obj));
        }
    }
}

