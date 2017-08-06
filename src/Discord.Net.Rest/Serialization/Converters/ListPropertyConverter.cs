using System.Collections.Generic;
using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class ListPropertyConverter<T> : IPropertyConverter<List<T>>
    {
        private readonly IPropertyConverter<T> _innerConverter;

        public ListPropertyConverter(IPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public List<T> ReadJson(JsonReader reader, bool read = true)
        {
            if ((read && !reader.Read()) || reader.TokenType != JsonTokenType.StartArray)
                throw new SerializationException("Bad input, expected StartArray");

            var list = new List<T>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                list.Add(_innerConverter.ReadJson(reader));
            return list;
        }

        public void WriteJson(JsonWriter writer, List<T> value)
        {
            writer.WriteArrayStart();
            for (int i = 0; i < value.Count; i++)
                _innerConverter.WriteJson(writer, value[i]);
            writer.WriteArrayEnd();
        }
    }
}
