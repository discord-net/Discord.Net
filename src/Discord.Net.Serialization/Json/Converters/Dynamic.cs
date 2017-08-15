using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    //TODO: Only supports cases where the key arrives first
    public class DynamicPropertyConverter : JsonPropertyConverter<object>
    {
        public override object Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (propMap.GetDynamicConverter(model, false) is IJsonPropertyReader<object> converter)
                return converter.Read(serializer, modelMap, propMap, model, ref reader, isTopLevel);
            else
            {
                JsonReaderUtils.Skip(ref reader);
                return null;
            }
        }

        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, object value, string key)
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
                var converter = (IJsonPropertyWriter)propMap.GetDynamicConverter(model, true);
                converter.Write(serializer, modelMap, propMap, model, ref writer, value, key);
            }
        }
    }
}
