using System.Collections.Generic;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class ArrayPropertyConverter<T> : JsonPropertyConverter<T[]>
    {
        private readonly JsonPropertyConverter<T> _innerConverter;

        public ArrayPropertyConverter(JsonPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override T[] Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if ((isTopLevel && !reader.Read()) || reader.TokenType != JsonTokenType.StartArray)
                throw new SerializationException("Bad input, expected StartArray");

            var list = new List<T>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return list.ToArray();
                list.Add(_innerConverter.Read(serializer, modelMap, propMap, model, ref reader, false));
            }
            throw new SerializationException("Bad input, expected EndArray");
        }

        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, T[] value, string key)
        {
            if (key != null)
                writer.WriteArrayStart(key);
            else
                writer.WriteArrayStart();
            for (int i = 0; i < value.Length; i++)
                _innerConverter.Write(serializer, modelMap, propMap, model, ref writer, value[i], null);
            writer.WriteArrayEnd();
        }
    }

    public class ListPropertyConverter<T> : JsonPropertyConverter<List<T>>
    {
        private readonly JsonPropertyConverter<T> _innerConverter;

        public ListPropertyConverter(JsonPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override List<T> Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if ((isTopLevel && !reader.Read()) || reader.TokenType != JsonTokenType.StartArray)
                throw new SerializationException("Bad input, expected StartArray");

            var list = new List<T>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return list;
                list.Add(_innerConverter.Read(serializer, modelMap, propMap, model, ref reader, false));
            }
            throw new SerializationException("Bad input, expected EndArray");
        }

        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, List<T> value, string key)
        {
            if (key != null)
                writer.WriteArrayStart(key);
            else
                writer.WriteArrayStart();
            for (int i = 0; i < value.Count; i++)
                _innerConverter.Write(serializer, modelMap, propMap, model, ref writer, value[i], null);
            writer.WriteArrayEnd();
        }
    }
}
