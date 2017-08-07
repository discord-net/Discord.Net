using System;
using System.Text.Formatting;

namespace Discord.Serialization
{
    public class Serializer
    {
        private readonly static Lazy<Serializer> _json = new Lazy<Serializer>(() => new Serializer(SerializationFormat.Json));
        public static Serializer Json => _json.Value;

        public event Action<Exception> Error; //TODO: Impl

        private readonly SerializationFormat _format;

        public Serializer(SerializationFormat format)
        {
            _format = format;
        }

        public T Read<T>(ReadOnlyBuffer<byte> data)
            where T : class, new()
            => _format.Read<T>(this, data);
        public void Write<T>(ArrayFormatter data, T obj)
            where T : class, new()
            => _format.Write(this, data, obj);
    }
}
