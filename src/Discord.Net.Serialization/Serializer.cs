using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Formatting;
using System.Text.Utf8;

namespace Discord.Serialization
{
    public abstract class Serializer
    {
        public event Action<string, Exception> ModelError;
        public event Action<string> UnmappedProperty;

        private static readonly MethodInfo _createPropertyMapMethod
            = typeof(Serializer).GetTypeInfo().GetDeclaredMethod(nameof(CreatePropertyMap));
        
        private readonly ConcurrentDictionary<Type, object> _maps;
        private readonly ConverterCollection _converters;
        private readonly ConcurrentHashSet<string> _unknownProps = new ConcurrentHashSet<string>();

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

        protected internal ModelMap MapModel<TModel>()
        {
            return _maps.GetOrAdd(typeof(TModel), _ =>
            {
                var type = typeof(TModel).GetTypeInfo();
                var searchType = type;
                var properties = new List<PropertyMap>();
                var map = new ModelMap(type.Name);

                while (searchType != null)
                {
                    var propInfos = searchType.DeclaredProperties
                        .Where(x => x.CanRead && x.CanWrite)
                        .ToArray();

                    for (int i = 0; i < propInfos.Length; i++)
                    {
                        if (propInfos[i].GetCustomAttribute<ModelPropertyAttribute>() != null)
                        {
                            var propMap = MapProperty<TModel>(map, propInfos[i]);
                            map.AddProperty(propMap);
                        }
                    }

                    searchType = searchType.BaseType?.GetTypeInfo();
                }
                return map;
            }) as ModelMap;
        }

        private PropertyMap MapProperty<TModel>(ModelMap modelMap, PropertyInfo propInfo)
            => _createPropertyMapMethod.MakeGenericMethod(typeof(TModel), propInfo.PropertyType).Invoke(this, new object[] { modelMap, propInfo }) as PropertyMap;
        protected abstract PropertyMap CreatePropertyMap<TModel, TValue>(ModelMap modelMap, PropertyInfo propInfo);

        public TModel Read<TModel>(ReadOnlyBuffer<byte> data)
            => Read<TModel>(data.Span);
        public abstract TModel Read<TModel>(ReadOnlySpan<byte> data);
        public abstract void Write<TModel>(ArrayFormatter stream, TModel model);

        internal void RaiseModelError(string path, Exception ex)
        {
            if (ModelError != null)
                ModelError?.Invoke(path, ex);
        }
        internal void RaiseModelError(ModelMap modelMap, Exception ex)
        {
            if (ModelError != null)
                ModelError?.Invoke(modelMap.Path, ex);
        }
        internal void RaiseModelError(PropertyMap propMap, Exception ex)
        {
            if (ModelError != null)
                ModelError?.Invoke(propMap.Path, ex);
        }

        internal void RaiseUnmappedProperty(string model, string propertyMap)
        {
            if (UnmappedProperty != null)
            {
                string path = $"{model}.{propertyMap}";
                if (_unknownProps.TryAdd(path))
                    UnmappedProperty?.Invoke(path);
            }
        }
        internal void RaiseUnknownProperty(string model, ReadOnlyBuffer<byte> propertyMap)
        {
            if (UnmappedProperty != null)
            {
                string path = $"{model}.{new Utf8String(propertyMap.Span).ToString()}";
                if (_unknownProps.TryAdd(path))
                    UnmappedProperty?.Invoke(path);
            }
        }
        internal void RaiseUnknownProperty(string model, ReadOnlySpan<byte> propertyMap)
        {
            if (UnmappedProperty != null)
            {
                string path = $"{model}.{new Utf8String(propertyMap).ToString()}";
                if (_unknownProps.TryAdd(path))
                    UnmappedProperty?.Invoke(path);
            }
        }
    }
}
