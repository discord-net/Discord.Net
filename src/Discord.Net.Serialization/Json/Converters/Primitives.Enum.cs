using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class EnumPropertyConverter<T> : IJsonPropertyConverter<T>
    {
        public T Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
            => throw new System.NotImplementedException();
        public void Write(PropertyMap map, ref JsonWriter writer, T value, bool isTopLevel)
            => throw new System.NotImplementedException();
    }
}
