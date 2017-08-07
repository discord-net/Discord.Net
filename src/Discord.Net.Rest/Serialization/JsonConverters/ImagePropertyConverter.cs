using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class ImagePropertyConverter : IJsonPropertyConverter<API.Image>
    {
        public API.Image Read(PropertyMap map, JsonReader reader, bool isTopLevel)
            => throw new System.NotImplementedException();
        public void Write(PropertyMap map, JsonWriter writer, API.Image value, bool isTopLevel)
            => throw new System.NotImplementedException();
    }
}
