using System.Text.Json;

namespace Discord.Serialization.Json
{
    public interface IJsonPropertyConverter<T>
    {
        T Read(PropertyMap map, ref JsonReader reader, bool isTopLevel);
        void Write(PropertyMap map, ref JsonWriter writer, T value, bool isTopLevel);
    }
}
