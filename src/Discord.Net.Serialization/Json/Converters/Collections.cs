using System.Collections.Generic;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class ListPropertyConverter<T> : IJsonPropertyConverter<List<T>>
    {
        private readonly IJsonPropertyConverter<T> _innerConverter;

        public ListPropertyConverter(IJsonPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public List<T> Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if ((isTopLevel && !reader.Read()) || reader.TokenType != JsonTokenType.StartArray)
                throw new SerializationException("Bad input, expected StartArray");

            var list = new List<T>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                list.Add(_innerConverter.Read(map, ref reader, false));
            return list;
        }

        public void Write(PropertyMap map, ref JsonWriter writer, List<T> value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteArrayStart(map.Key);
            else
                writer.WriteArrayStart();
            for (int i = 0; i < value.Count; i++)
                _innerConverter.Write(map, ref writer, value[i], false);
            writer.WriteArrayEnd();
        }
    }
}
