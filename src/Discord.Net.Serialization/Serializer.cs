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

        protected object GetConverter(Type type, PropertyInfo propInfo = null)
            => _converters.Get(type, propInfo);

        public void AddConverter(Type type, Type converter)
        {
            CheckScoped();
            _converters.Add(type, converter);
        }
        public void AddConverter(Type type, Type converter, Func<TypeInfo, PropertyInfo, bool> condition)
        {
            CheckScoped();
            _converters.Add(type, converter, condition);
        }

        public void AddGenericConverter(Type converter)
        {
            CheckScoped();
            _converters.AddGeneric(converter);
        }
        public void AddGenericConverter(Type converter, Func<TypeInfo, PropertyInfo, bool> condition)
        {
            CheckScoped();
            _converters.AddGeneric(converter, condition);
        }
        public void AddGenericConverter(Type value, Type converter)
        {
            CheckScoped();
            _converters.AddGeneric(value, converter);
        }
        public void AddGenericConverter(Type value, Type converter, Func<TypeInfo, PropertyInfo, bool> condition)
        {
            CheckScoped();
            _converters.AddGeneric(value, converter, condition);
        }

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
                    var propMap = MapProperty<TModel>(propInfos[i]);
                    properties.Add(propMap);
                }
                return new ModelMap<TModel>(properties);
            }) as ModelMap<TModel>;
        }

        private PropertyMap MapProperty<TModel>(PropertyInfo propInfo)
            where TModel : class, new()
            => _createPropertyMapMethod.MakeGenericMethod(typeof(TModel), propInfo.PropertyType).Invoke(this, new object[] { propInfo }) as PropertyMap;
        protected abstract PropertyMap CreatePropertyMap<TModel, TValue>(PropertyInfo propInfo)
            where TModel : class, new();

        public abstract TModel Read<TModel>(ReadOnlyBuffer<byte> data)
            where TModel : class, new();
        public abstract void Write<TModel>(ArrayFormatter stream, TModel model)
            where TModel : class, new();

        private void CheckScoped()
        {
            if (IsScoped)
                throw new InvalidOperationException("Scoped serializers are read-only");
        }
    }
}
