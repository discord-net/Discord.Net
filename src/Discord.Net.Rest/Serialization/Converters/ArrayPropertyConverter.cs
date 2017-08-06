using System.Collections.Generic;
using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class ArrayPropertyConverter<T> : IPropertyConverter<T[]>
    {
        private readonly IPropertyConverter<T> _innerConverter;

        public ArrayPropertyConverter(IPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public T[] ReadJson(JsonReader reader, bool read = true)
        {
            if ((read && !reader.Read()) || reader.TokenType != JsonTokenType.StartArray)
                throw new SerializationException("Bad input, expected StartArray");

            var list = new List<T>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                list.Add(_innerConverter.ReadJson(reader));
            return list.ToArray();
        }

        public void WriteJson(JsonWriter writer, T[] value)
        {
            writer.WriteArrayStart();
            for (int i = 0; i < value.Length; i++)
                _innerConverter.WriteJson(writer, value[i]);
            writer.WriteArrayEnd();
        }
    }
}
