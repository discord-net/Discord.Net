using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class EnumPropertyConverter<T> : IJsonPropertyConverter<T>
        where T : struct
    {
        private static readonly EnumMap<T> _map = EnumMap.For<T>();

        public T Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return _map.GetValue(reader.Value);
        }
        public void Write(PropertyMap map, ref JsonWriter writer, T value, bool isTopLevel)
        {
            string key = _map.GetKey(value);
            if (isTopLevel)
                writer.WriteAttribute(map.Key, key);
            else
                writer.WriteValue(key);
        }
    }
}