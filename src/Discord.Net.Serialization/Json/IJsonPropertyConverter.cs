using System.Text.Json;

namespace Discord.Serialization.Json
{
    public interface IJsonPropertyConverter<T>
    {
        T Read(PropertyMap map, JsonReader reader, bool isTopLevel);
        void Write(PropertyMap map, JsonWriter writer, T value, bool isTopLevel);
    }
}
