using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Discord.Serialization
{
    internal static class ModelMap
    {
        private static readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        internal static ModelMap<T> For<T>()
            where T : class, new()
        {
            return _cache.GetOrAdd(typeof(T), _ =>
            {
                var type = typeof(T).GetTypeInfo();
                var properties = new Dictionary<string, PropertyMap<T>>();

                var propInfos = type.DeclaredProperties.ToArray();
                for (int i = 0; i < propInfos.Length; i++)
                {
                    var propInfo = propInfos[i];
                    if (!propInfo.CanRead || !propInfo.CanWrite)
                        continue;

                    var propMap = PropertyMap.Create<T>(propInfo);
                    properties.Add(propMap.Key, propMap);
                }

                return new ModelMap<T>(properties);
            }) as ModelMap<T>;
        }
    }

    internal class ModelMap<T>
        where T : class, new()
    {
        private readonly PropertyMap<T>[] _propertyList;
        private readonly Dictionary<string, PropertyMap<T>> _properties;

        public ModelMap(Dictionary<string, PropertyMap<T>> properties)
        {
            _properties = properties;
            _propertyList = _properties.Values.ToArray();
        }

        public T ReadJson(JsonReader reader)
        {
            var model = new T();

            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                throw new InvalidOperationException("Bad input, expected StartObject");
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return model;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new InvalidOperationException("Bad input, expected PropertyName");

                string key = reader.GetString();
                if (_properties.TryGetValue(key, out var property))
                    property.ReadJson(model, reader);
                else
                    reader.Skip(); //Unknown property, skip

                if (!reader.Read())
                    throw new InvalidOperationException("Bad input, expected Value");
            }
            throw new InvalidOperationException("Bad input, expected EndObject");
        }
        public void WriteJson(T model, JsonWriter writer)
        {
            writer.WriteObjectStart();
            for (int i = 0; i < _propertyList.Length; i++)
            {
                var property = _propertyList[i];
                writer.WriteStartAttribute(property.Key);
                property.WriteJson(model, writer);
            }
            writer.WriteObjectEnd();
        }
    }
}
