using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class Int64EnumPropertyConverter<T> : JsonPropertyConverter<T>
        where T : struct
    {
        private static readonly EnumMap<T> _map = EnumMap.For<T>();

        public override T Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return _map.FromInt64(reader.Value);
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, T value, string key)
        {
            long intVal = _map.ToInt64(value);
            if (key != null)
                writer.WriteAttribute(key, intVal);
            else
                writer.WriteValue(intVal);
        }
    }

    public class UInt64EnumPropertyConverter<T> : JsonPropertyConverter<T>
        where T : struct
    {
        private static readonly EnumMap<T> _map = EnumMap.For<T>();

        public override T Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return _map.FromUInt64(reader.Value);
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, T value, string key)
        {
            ulong uintVal = _map.ToUInt64(value);
            if (key != null)
                writer.WriteAttribute(key, uintVal);
            else
                writer.WriteValue(uintVal);
        }
    }

    public class StringEnumPropertyConverter<T> : JsonPropertyConverter<T>
        where T : struct
    {
        private static readonly EnumMap<T> _map = EnumMap.For<T>();

        public override T Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return _map.FromKey(reader.Value);
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, T value, string key)
        {
            string strVal = _map.ToUtf16Key(value);
            if (key != null)
                writer.WriteAttribute(key, strVal);
            else
                writer.WriteValue(strVal);
        }
    }
}