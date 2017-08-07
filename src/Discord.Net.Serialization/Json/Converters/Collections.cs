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

        public List<T> Read(JsonReader reader, bool read = true)
        {
            if ((read && !reader.Read()) || reader.TokenType != JsonTokenType.StartArray)
                throw new SerializationException("Bad input, expected StartArray");

            var list = new List<T>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                list.Add(_innerConverter.Read(reader));
            return list;
        }

        public void Write(JsonWriter writer, List<T> value)
        {
            writer.WriteArrayStart();
            for (int i = 0; i < value.Count; i++)
                _innerConverter.Write(writer, value[i]);
            writer.WriteArrayEnd();
        }
    }
}
