using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal interface IPropertyConverter<T>
    {
        T ReadJson(JsonReader reader, bool read = true);
        void WriteJson(JsonWriter writer, T value);
    }
}
