using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class ImagePropertyConverter : IJsonPropertyConverter<API.Image>
    {
        public API.Image Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
            => throw new System.NotImplementedException();
        public void Write(PropertyMap map, ref JsonWriter writer, API.Image value, bool isTopLevel)
            => throw new System.NotImplementedException();
    }
}
