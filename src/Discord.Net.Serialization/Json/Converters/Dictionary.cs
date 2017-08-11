using System.Collections.Generic;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class DictionaryPropertyConverter<TValue> : JsonPropertyConverter<Dictionary<string, TValue>>
    {
        private readonly JsonPropertyConverter<TValue> _valueConverter;

        public DictionaryPropertyConverter(JsonPropertyConverter<TValue> valueConverter)
        {
            _valueConverter = valueConverter;
        }

        public override Dictionary<string, TValue> Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if ((isTopLevel && !reader.Read()) || reader.TokenType != JsonTokenType.StartObject)
                throw new SerializationException("Bad input, expected StartObject");

            var dic = new Dictionary<string, TValue>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dic;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new SerializationException("Bad input, expected PropertyName");

                string key = reader.Value.ParseString();
                var value = _valueConverter.Read(map, model, ref reader, false);
                dic.Add(key, value);
            }
            return dic;
        }

        public override void Write(PropertyMap map, object model, ref JsonWriter writer, Dictionary<string, TValue> value, string key)
        {
            if (key != null)
                writer.WriteObjectStart(key);
            else
                writer.WriteObjectStart();
            foreach (var pair in value)
                _valueConverter.Write(map, model, ref writer, pair.Value, pair.Key);
            writer.WriteObjectEnd();
        }
    }
}
