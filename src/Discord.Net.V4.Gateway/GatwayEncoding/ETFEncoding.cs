using System;

namespace Discord.Gateway
{
    public sealed class ETFEncoding : IGatewayEncoding
    {
        private readonly DiscordGatewayConfig _config;

        public ETFEncoding(DiscordGatewayConfig config)
        {
            _config = config;
        }

        public T Decode<T>(Stream data)
            => ETF.DecodeObject<T>(data);

        public ReadOnlyMemory<byte> Encode<T>(T value)
            => ETF.EncodeObject(value, _config.BufferPool);

        public T? ToObject<T>(object? obj)
        {
            if (obj is null)
                return default;

            if (obj is ETFObject etfObj)
                return etfObj.ToObject<T>();

            throw new ArgumentException("Argument is not sourced from ETF", nameof(obj));
        }
    }
}

