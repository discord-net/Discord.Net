using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Serialization
{
    internal class ConverterCollection
    {
        private class ConverterTypeCollection
        {
            public Type DefaultConverterType;
            public List<(Func<TypeInfo, PropertyInfo, bool> Condition, Type ConverterType)> Conditionals = new List<(Func<TypeInfo, PropertyInfo, bool>, Type)>();
        }

        private static readonly TypeInfo _serializerType = typeof(Serializer).GetTypeInfo();

        private readonly Serializer _serializer;
        private readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();
        private readonly Dictionary<Type, ConverterTypeCollection> _types = new Dictionary<Type, ConverterTypeCollection>();
        private readonly Dictionary<Type, ConverterTypeCollection> _mappedGenericTypes = new Dictionary<Type, ConverterTypeCollection>();
        private readonly ConverterTypeCollection _genericTypes = new ConverterTypeCollection();

        internal ConverterCollection(Serializer serializer)
        {
            _serializer = serializer;
        }

        public void Add(Type type, Type converterType)
        {
            if (!_types.TryGetValue(type, out var converters))
                _types.Add(type, converters = new ConverterTypeCollection());
            converters.DefaultConverterType = converterType;
        }
        public void Add(Type type, Type converterType, Func<TypeInfo, PropertyInfo, bool> condition)
        {
            if (!_types.TryGetValue(type, out var converters))
                _types.Add(type, converters = new ConverterTypeCollection());
            converters.Conditionals.Add((condition, converterType));
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
        
        public object Get(Type type, PropertyInfo propInfo = null)
        {
            if (!_cache.TryGetValue(type, out var result))
            {
                object converter = Create(type, propInfo);
                result = _cache.GetOrAdd(type, converter);
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
                    object innerConverter = Get(innerType, propInfo);
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
                    var converterTypeInfo = converterType.GetTypeInfo();

                    var constructors = converterTypeInfo.DeclaredConstructors.ToArray();
                    if (constructors.Length == 0)
                        throw new SerializationException($"{converterType.Name} is missing a constructor");
                    if (constructors.Length != 1)
                        throw new SerializationException($"{converterType.Name} has multiple constructors");
                    var constructor = constructors[0];
                    var parameters = constructor.GetParameters();

                    if (parameters.Length == 0)
                        return constructor.Invoke(null);
                    else if (parameters.Length == 1)
                    {
                        var parameterType = parameters[0].ParameterType.GetTypeInfo();
                        if (_serializerType.IsAssignableFrom(parameterType))
                            return constructor.Invoke(new object[] { _serializer });
                    }
                    throw new SerializationException($"{converterType.Name} has an unsupported constructor");
                }
            }

            throw new InvalidOperationException($"Unsupported model type: {type.Name}");
        }

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
