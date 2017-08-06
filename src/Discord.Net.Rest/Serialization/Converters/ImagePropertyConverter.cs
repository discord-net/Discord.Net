using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class ImagePropertyConverter : IPropertyConverter<Image>
    {
        public Image ReadJson(JsonReader reader, bool read = true) 
            => throw new System.NotImplementedException();
        public void WriteJson(JsonWriter writer, Image value) 
            => throw new System.NotImplementedException();
    }
}
