using Discord.Serialization.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Formatting;

namespace Discord.Serialization
{
    public abstract class SerializationFormat
    {
        private static readonly MethodInfo _getConverterMethod
            = typeof(SerializationFormat).GetTypeInfo().GetDeclaredMethod(nameof(CreatePropertyMap));

        private static readonly Lazy<JsonFormat> _json = new Lazy<JsonFormat>(() => new JsonFormat());
        public static JsonFormat Json => _json.Value;

        protected readonly ConcurrentDictionary<Type, object> _maps = new ConcurrentDictionary<Type, object>();
        protected readonly ConverterCollection _converters = new ConverterCollection();

        protected internal ModelMap<TModel> MapModel<TModel>()
            where TModel : class, new()
        {
            return _maps.GetOrAdd(typeof(TModel), _ =>
            {
                var type = typeof(TModel).GetTypeInfo();
                var propInfos = type.DeclaredProperties
                    .Where(x => x.CanRead && x.CanWrite)
                    .ToArray();

                var properties = new Dictionary<ReadOnlySpan<byte>, PropertyMap>(propInfos.Length, Utf8SpanComparer.Instance);
                for (int i = 0; i < propInfos.Length; i++)
                {
                    var propMap = MapProperty<TModel>(propInfos[i]);
                    properties.Add(propMap.Utf8Key, propMap);
                }
                return new ModelMap<TModel>(properties);
            }) as ModelMap<TModel>;
        }

        private PropertyMap MapProperty<TModel>(PropertyInfo propInfo)
            where TModel : class, new()
            => _getConverterMethod.MakeGenericMethod(typeof(TModel), propInfo.PropertyType).Invoke(this, new object[] { propInfo }) as PropertyMap;


        protected internal abstract TModel Read<TModel>(Serializer serializer, ReadOnlyBuffer<byte> data)
            where TModel : class, new();
        protected internal abstract void Write<TModel>(Serializer serializer, ArrayFormatter stream, TModel model)
            where TModel : class, new();

        protected abstract PropertyMap CreatePropertyMap<TModel, TValue>(PropertyInfo propInfo)
            where TModel : class, new();
    }
}
