using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    //TODO: Only supports cases where the key arrives first
    public class DynamicPropertyConverter : JsonPropertyConverter<object>
    {
        public override object Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (map.GetDynamicConverter(model, false) is IJsonPropertyReader<object> converter)
                return converter.Read(map, model, ref reader, isTopLevel);
            else
            {
                JsonReaderUtils.Skip(ref reader);
                return null;
            }
        }

        public override void Write(PropertyMap map, object model, ref JsonWriter writer, object value, string key)
        {
            if (value == null)
            {
                if (key != null)
                    writer.WriteAttributeNull(key);
                else
                    writer.WriteNull();
            }
            else
            {
                var converter = (IJsonPropertyWriter)map.GetDynamicConverter(model, true);
                converter.Write(map, model, ref writer, value, key);
            }
        }
    }
}
