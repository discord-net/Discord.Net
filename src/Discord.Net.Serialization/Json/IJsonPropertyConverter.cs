using System.Text.Json;

namespace Discord.Serialization.Json
{
    public interface IJsonPropertyConverter<T> : IJsonPropertyReader<T>, IJsonPropertyWriter<T> { }

    public interface IJsonPropertyReader<out T>
    {
        T Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel);
    }
    public interface IJsonPropertyWriter<in T>
    {
        void Write(PropertyMap map, object model, ref JsonWriter writer, T value, bool isTopLevel);
    }
}
