using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Formatting;

namespace Discord.Serialization
{
    public abstract class Serializer
    {
        public event Action<Exception> Error; //TODO: Impl

        private static readonly MethodInfo _createPropertyMapMethod
            = typeof(Serializer).GetTypeInfo().GetDeclaredMethod(nameof(CreatePropertyMap));
        
        private readonly ConcurrentDictionary<Type, object> _maps;
        private readonly ConverterCollection _converters;

        protected Serializer()
            : this(null) { }
        protected Serializer(Serializer parent)
        {
            _maps = new ConcurrentDictionary<Type, object>();
            _converters = new ConverterCollection(parent?._converters);
        }

        protected internal object GetConverter(Type valueType, PropertyInfo propInfo = null)
            => _converters.Get(this, valueType, propInfo);

        public void AddConverter(Type valueType, Type converterType)
            => _converters.Add(valueType, converterType);
        public void AddConverter(Type valueType, Type converterType, Func<TypeInfo, PropertyInfo, bool> condition)
            => _converters.Add(valueType, converterType, condition);

        public void AddGenericConverter(Type converterType, Func<Type, Type> typeSelector = null)
            => _converters.AddGeneric(converterType, typeSelector);
        public void AddGenericConverter(Type converterType, Func<TypeInfo, PropertyInfo, bool> condition, Func<Type, Type> typeSelector = null)
            => _converters.AddGeneric(converterType, condition, typeSelector);
        public void AddGenericConverter(Type valueType, Type converterType, Func<Type, Type> typeSelector = null)
            => _converters.AddGeneric(valueType, converterType, typeSelector);
        public void AddGenericConverter(Type valueType, Type converterType, Func<TypeInfo, PropertyInfo, bool> condition, Func<Type, Type> typeSelector = null)
            => _converters.AddGeneric(valueType, converterType, condition, typeSelector);

        public void AddSelectorConverter(string groupKey, Type keyType, object keyValue, Type converterType)
            => _converters.AddSelector(this, groupKey, keyType, keyValue, converterType);
        public void AddSelectorConverter<TKey, TConverter>(string groupKey, TKey keyValue)
            => _converters.AddSelector(this, groupKey, typeof(TKey), keyValue, typeof(TConverter));
        
        public ISelectorGroup GetSelectorGroup(Type keyType, string groupKey)
            => _converters.GetSelectorGroup(keyType, groupKey);

        protected internal ModelMap<TModel> MapModel<TModel>()
            where TModel : class, new()
        {
            return _maps.GetOrAdd(typeof(TModel), _ =>
            {
                var type = typeof(TModel).GetTypeInfo();
                var properties = new List<PropertyMap>();
                while (type != null)
                {
                    var propInfos = type.DeclaredProperties
                        .Where(x => x.CanRead && x.CanWrite)
                        .ToArray();

                    for (int i = 0; i < propInfos.Length; i++)
                    {
                        if (propInfos[i].GetCustomAttribute<ModelPropertyAttribute>() != null)
                        {
                            var propMap = MapProperty<TModel>(propInfos[i]);
                            properties.Add(propMap);
                        }
                    }

                    type = type.BaseType?.GetTypeInfo();
                }
                return new ModelMap<TModel>(this, type, properties);
            }) as ModelMap<TModel>;
        }

        private PropertyMap MapProperty<TModel>(PropertyInfo propInfo)
            => _createPropertyMapMethod.MakeGenericMethod(typeof(TModel), propInfo.PropertyType).Invoke(this, new object[] { propInfo }) as PropertyMap;
        protected abstract PropertyMap CreatePropertyMap<TModel, TValue>(PropertyInfo propInfo);

        public TModel Read<TModel>(ReadOnlyBuffer<byte> data)
            => Read<TModel>(data.Span);
        public abstract TModel Read<TModel>(ReadOnlySpan<byte> data);
        public abstract void Write<TModel>(ArrayFormatter stream, TModel model);
    }
}
