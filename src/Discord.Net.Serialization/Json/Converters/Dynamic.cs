using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    //TODO: Only supports cases where the key arrives first
    public class DynamicPropertyConverter : IJsonPropertyConverter<object>
    {
        public object Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (map.GetDynamicConverter(model, false) is IJsonPropertyReader<object> converter)
                return converter.Read(map, model, ref reader, isTopLevel);
            else
            {
                JsonReaderUtils.Skip(ref reader);
                return null;
            }
        }

        public void Write(PropertyMap map, object model, ref JsonWriter writer, object value, bool isTopLevel)
        {
            if (value == null)
            {
                if (isTopLevel)
                    writer.WriteAttributeNull(map.Key);
                else
                    writer.WriteNull();
            }
            else
            {
                var converter = map.GetDynamicConverter(model, true) as IJsonPropertyWriter<object>;
                converter.Write(map, model, ref writer, value, isTopLevel);
            }
        }
    }
}
