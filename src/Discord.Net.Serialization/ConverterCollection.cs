using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Serialization
{
    public class ConverterCollection
    {
        private class ConverterTypeCollection
        {
            public Type DefaultConverterType;
            public List<(Func<TypeInfo, PropertyInfo, bool> Condition, Type ConverterType)> Conditionals = new List<(Func<TypeInfo, PropertyInfo, bool>, Type)>();
        }

        private static readonly MethodInfo _getConverterMethod
            = typeof(ConverterCollection).GetTypeInfo().GetDeclaredMethod(nameof(Get));

        private readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();
        private readonly Dictionary<Type, ConverterTypeCollection> _types = new Dictionary<Type, ConverterTypeCollection>();
        private readonly Dictionary<Type, ConverterTypeCollection> _mappedGenericTypes = new Dictionary<Type, ConverterTypeCollection>();
        private readonly ConverterTypeCollection _genericTypes = new ConverterTypeCollection();

        internal ConverterCollection() { }

        public void Add<TType, TConverter>()
        {
            if (!_types.TryGetValue(typeof(TType), out var converters))
                _types.Add(typeof(TType), converters = new ConverterTypeCollection());
            converters.DefaultConverterType = typeof(TConverter);
        }
        public void Add<TType, TConverter>(Func<TypeInfo, PropertyInfo, bool> condition)
        {
            if (!_types.TryGetValue(typeof(TType), out var converters))
                _types.Add(typeof(TType), converters = new ConverterTypeCollection());
            converters.Conditionals.Add((condition, typeof(TConverter)));
        }

        public void AddGeneric(Type openConverterType)
        {
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            _genericTypes.DefaultConverterType = openConverterType;
        }
        public void AddGeneric(Type openConverterType, Func<TypeInfo, PropertyInfo, bool> condition)
        {
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            _genericTypes.Conditionals.Add((condition, openConverterType));
        }
        public void AddGeneric(Type openType, Type openConverterType)
        {
            if (openType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openType)} must be an open generic");
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            if (!_mappedGenericTypes.TryGetValue(openType, out var converters))
                _mappedGenericTypes.Add(openType, converters = new ConverterTypeCollection());
            converters.DefaultConverterType = openConverterType;
        }
        public void AddGeneric(Type openType, Type openConverterType, Func<TypeInfo, PropertyInfo, bool> condition)
        {
            if (openType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openType)} must be an open generic");
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            if (!_mappedGenericTypes.TryGetValue(openType, out var converters))
                _mappedGenericTypes.Add(openType, converters = new ConverterTypeCollection());
            converters.Conditionals.Add((condition, openConverterType));
        }
        
        public object Get<TType>(PropertyInfo propInfo = null)
        {
            if (!_cache.TryGetValue(typeof(TType), out var result))
            {
                object converter = Create(typeof(TType), propInfo);
                result = _cache.GetOrAdd(typeof(TType), converter);
            }
            return result;
        }
        private object Create(Type type, PropertyInfo propInfo)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            //Mapped generic converters (List<T> -> CollectionPropertyConverter<T>)
            if (typeInfo.IsGenericType)
            {
                var converterType = FindConverterType(typeInfo.GetGenericTypeDefinition(), _mappedGenericTypes, typeInfo, propInfo);
                if (converterType != null)
                {
                    var innerType = typeInfo.GenericTypeArguments[0];
                    converterType = converterType.MakeGenericType(innerType);
                    object innerConverter = GetInnerConverter(innerType, propInfo);
                    return Activator.CreateInstance(converterType, innerConverter);
                }
            }

            //Normal converters (bool -> BooleanPropertyConverter)
            {
                var converterType = FindConverterType(type, _types, typeInfo, propInfo);
                if (converterType != null)
                    return Activator.CreateInstance(converterType);
            }

            //Generic converters (Model -> ObjectPropertyConverter<Model>)
            {
                var converterType = FindConverterType(_genericTypes, typeInfo, propInfo);
                if (converterType != null)
                {
                    converterType = converterType.MakeGenericType(type);
                    return Activator.CreateInstance(converterType);
                }
            }

            throw new InvalidOperationException($"Unsupported model type: {type.Name}");
        }
        private object GetInnerConverter(Type type, PropertyInfo propInfo)
            => _getConverterMethod.MakeGenericMethod(type).Invoke(this, new object[] { propInfo });

        private Type FindConverterType(Type type, Dictionary<Type, ConverterTypeCollection> collection, TypeInfo typeInfo, PropertyInfo propInfo)
        {
            if (collection.TryGetValue(type, out var converters))
                return FindConverterType(converters, typeInfo, propInfo);
            return null;
        }
        private Type FindConverterType(ConverterTypeCollection converters, TypeInfo typeInfo, PropertyInfo propInfo)
        {
            for (int i = 0; i < converters.Conditionals.Count; i++)
            {
                if (converters.Conditionals[i].Condition(typeInfo, propInfo))
                    return converters.Conditionals[i].ConverterType;
            }
            if (converters.DefaultConverterType != null)
                return converters.DefaultConverterType;
            return null;
        }
    }
}
