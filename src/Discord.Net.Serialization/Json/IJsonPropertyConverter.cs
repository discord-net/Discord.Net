using System.Text.Json;

namespace Discord.Serialization.Json
{
    public abstract class JsonPropertyConverter<T> : IJsonPropertyReader<T>, IJsonPropertyWriter<T>, IJsonPropertyWriter
    {
        public abstract T Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel);
        public abstract void Write(PropertyMap map, object model, ref JsonWriter writer, T value, string key);

        void IJsonPropertyWriter.Write(PropertyMap map, object model, ref JsonWriter writer, object value, string key)
            => Write(map, model, ref writer, (T)value, key);
    }

    public interface IJsonPropertyReader<out T>
    {
        T Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel);
    }
    public interface IJsonPropertyWriter<in T>
    {
        void Write(PropertyMap map, object model, ref JsonWriter writer, T value, string key);
    }
    public interface IJsonPropertyWriter
    {
        void Write(PropertyMap map, object model, ref JsonWriter writer, object value, string key);
    }
}
