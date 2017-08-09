using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class Int64EnumPropertyConverter<T> : IJsonPropertyConverter<T>
        where T : struct
    {
        private static readonly EnumMap<T> _map = EnumMap.For<T>();

        public T Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return _map.FromInt64(reader.Value);
        }
        public void Write(PropertyMap map, object model, ref JsonWriter writer, T value, bool isTopLevel)
        {
            long key = _map.ToInt64(value);
            if (isTopLevel)
                writer.WriteAttribute(map.Key, key);
            else
                writer.WriteValue(key);
        }
    }

    public class UInt64EnumPropertyConverter<T> : IJsonPropertyConverter<T>
        where T : struct
    {
        private static readonly EnumMap<T> _map = EnumMap.For<T>();

        public T Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return _map.FromUInt64(reader.Value);
        }
        public void Write(PropertyMap map, object model, ref JsonWriter writer, T value, bool isTopLevel)
        {
            ulong key = _map.ToUInt64(value);
            if (isTopLevel)
                writer.WriteAttribute(map.Key, key);
            else
                writer.WriteValue(key);
        }
    }

    public class StringEnumPropertyConverter<T> : IJsonPropertyConverter<T>
        where T : struct
    {
        private static readonly EnumMap<T> _map = EnumMap.For<T>();

        public T Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return _map.FromKey(reader.Value);
        }
        public void Write(PropertyMap map, object model, ref JsonWriter writer, T value, bool isTopLevel)
        {
            string key = _map.ToUtf16Key(value);
            if (isTopLevel)
                writer.WriteAttribute(map.Key, key);
            else
                writer.WriteValue(key);
        }
    }
}