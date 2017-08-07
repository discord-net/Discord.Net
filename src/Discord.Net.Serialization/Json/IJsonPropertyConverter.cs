using System.Text.Json;

namespace Discord.Serialization.Json
{
    public interface IJsonPropertyConverter<T>
    {
        T Read(JsonReader reader, bool read = true);
        void Write(JsonWriter writer, T value);
    }
}
