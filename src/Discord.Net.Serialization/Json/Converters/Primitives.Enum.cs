using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class EnumPropertyConverter<T> : IJsonPropertyConverter<T>
    {
        public T Read(JsonReader reader, bool read = true) 
            => throw new System.NotImplementedException();
        public void Write(JsonWriter writer, T value) 
            => throw new System.NotImplementedException();
    }
}
