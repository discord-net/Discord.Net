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

        public bool IsScoped { get; }

        protected Serializer()
        {
            _maps = new ConcurrentDictionary<Type, object>();
            _converters = new ConverterCollection(this);
            IsScoped = false;
        }
        protected Serializer(Serializer parent)
        {
            _maps = parent._maps;
            _converters = parent._converters;
            IsScoped = true;
        }

        protected object GetConverter(Type valueType, PropertyInfo propInfo = null)
            => _converters.Get(valueType, propInfo);

        public void AddConverter(Type valueType, Type converterType)
        {
            CheckScoped();
            _converters.Add(valueType, converterType);
        }
        public void AddConverter(Type valueType, Type converterType, Func<TypeInfo, PropertyInfo, bool> condition)
        {
            CheckScoped();
            _converters.Add(valueType, converterType, condition);
        }

        public void AddGenericConverter(Type converterType)
        {
            CheckScoped();
            _converters.AddGeneric(converterType);
        }
        public void AddGenericConverter(Type converterType, Func<TypeInfo, PropertyInfo, bool> condition)
        {
            CheckScoped();
            _converters.AddGeneric(converterType, condition);
        }
        public void AddGenericConverter(Type valueType, Type converterType)
        {
            CheckScoped();
            _converters.AddGeneric(valueType, converterType);
        }
        public void AddGenericConverter(Type valueType, Type converterType, Func<TypeInfo, PropertyInfo, bool> condition)
        {
            CheckScoped();
            _converters.AddGeneric(valueType, converterType, condition);
        }

        public void AddSelectorConverter(string groupKey, Type keyType, object keyValue, Type converterType)
        {
            CheckScoped();
            _converters.AddSelector(groupKey, keyType, keyValue, converterType);
        }
        public ISelectorGroup GetSelectorGroup(Type keyType, string groupKey)
            => _converters.GetSelectorGroup(keyType, groupKey);

        protected internal ModelMap<TModel> MapModel<TModel>()
            where TModel : class, new()
        {
            return _maps.GetOrAdd(typeof(TModel), _ =>
            {
                var type = typeof(TModel).GetTypeInfo();
                var propInfos = type.DeclaredProperties
                    .Where(x => x.CanRead && x.CanWrite)
                    .ToArray();

                var properties = new List<PropertyMap>();
                for (int i = 0; i < propInfos.Length; i++)
                {
                    if (propInfos[i].GetCustomAttribute<ModelPropertyAttribute>() != null)
                    {
                        var propMap = MapProperty<TModel>(propInfos[i]);
                        properties.Add(propMap);
                    }
                }
                return new ModelMap<TModel>(this, type, properties);
            }) as ModelMap<TModel>;
        }

        private PropertyMap MapProperty<TModel>(PropertyInfo propInfo)
            => _createPropertyMapMethod.MakeGenericMethod(typeof(TModel), propInfo.PropertyType).Invoke(this, new object[] { propInfo }) as PropertyMap;
        protected abstract PropertyMap CreatePropertyMap<TModel, TValue>(PropertyInfo propInfo);

        public abstract TModel Read<TModel>(ReadOnlyBuffer<byte> data);
        public abstract void Write<TModel>(ArrayFormatter stream, TModel model);
        
        private void CheckScoped()
        {
            if (IsScoped)
                throw new InvalidOperationException("Scoped serializers are read-only");
        }
    }
}
