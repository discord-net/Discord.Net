using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class EnumPropertyConverter<T> : IPropertyConverter<T>
    {
        public T ReadJson(JsonReader reader, bool read = true) 
            => throw new System.NotImplementedException();
        public void WriteJson(JsonWriter writer, T value) 
            => throw new System.NotImplementedException();
    }
}
