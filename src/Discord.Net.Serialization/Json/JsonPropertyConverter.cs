using System;
using System.Text.Json;

namespace Discord.Serialization.Json
{
    public abstract class JsonPropertyConverter<T> : IJsonPropertyReader<T>, IJsonPropertyWriter<T>, IJsonPropertyWriter
    {
        public abstract T Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel);
        public abstract void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, T value, string key);

        void IJsonPropertyWriter.Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, object value, string key)
            => Write(serializer, modelMap, propMap, model, ref writer, (T)value, key);

        protected void RaiseUnmappedProperty(Serializer serializer, ModelMap modelMap, ReadOnlyBuffer<byte> propertyKey)
            => serializer.RaiseUnknownProperty(modelMap.Path, propertyKey);
        protected void RaiseUnmappedProperty(Serializer serializer, ModelMap modelMap, ReadOnlySpan<byte> propertyKey)
            => serializer.RaiseUnknownProperty(modelMap.Path, propertyKey);
        protected void RaiseModelError(Serializer serializer, PropertyMap propMap, Exception ex)
            => serializer.RaiseModelError(propMap.Path, ex);
    }

    public interface IJsonPropertyReader<out T>
    {
        T Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel);
    }
    public interface IJsonPropertyWriter<in T>
    {
        void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, T value, string key);
    }
    public interface IJsonPropertyWriter
    {
        void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, object value, string key);
    }
}
