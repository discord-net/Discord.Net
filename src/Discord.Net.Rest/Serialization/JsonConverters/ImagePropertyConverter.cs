using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class ImagePropertyConverter : IJsonPropertyConverter<API.Image>
    {
        public API.Image Read(JsonReader reader, bool read = true) 
            => throw new System.NotImplementedException();
        public void Write(JsonWriter writer, API.Image value) 
            => throw new System.NotImplementedException();
    }
}
