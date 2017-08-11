using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Serialization
{
    internal class ConverterCollection
    {
        private class GenericConverterType
        {
            public Type Type;
            public Func<Type, Type> InnerTypeSelector;

            public GenericConverterType(Type type, Func<Type, Type> innerTypeSelector)
            {
                Type = type;
                InnerTypeSelector = innerTypeSelector ?? (x => x);
            }
        }
        private class ConverterTypeCollection
        {
            public Type DefaultConverterType;
            public List<(Func<TypeInfo, PropertyInfo, bool> Condition, Type ConverterType)> Conditionals 
                = new List<(Func<TypeInfo, PropertyInfo, bool>, Type)>();
        }
        private class GenericConverterTypeCollection
        {
            public GenericConverterType DefaultConverterType;
            public List<(Func<TypeInfo, PropertyInfo, bool> Condition, GenericConverterType ConverterType)> Conditionals
                = new List<(Func<TypeInfo, PropertyInfo, bool>, GenericConverterType)>();
        }

        private static readonly TypeInfo _serializerType = typeof(Serializer).GetTypeInfo();

        private readonly ConverterCollection _parent;
        private readonly ConcurrentDictionary<Type, object> _cache;
        private readonly Dictionary<Type, ConverterTypeCollection> _types;
        private readonly Dictionary<Type, GenericConverterTypeCollection> _mappedGenericTypes;
        private readonly GenericConverterTypeCollection _genericTypes;
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, ISelectorGroup>> _selectorGroups;

        internal ConverterCollection(ConverterCollection parent = null)
        {
            _parent = parent;
            _cache = new ConcurrentDictionary<Type, object>();
            _types = new Dictionary<Type, ConverterTypeCollection>();
            _mappedGenericTypes = new Dictionary<Type, GenericConverterTypeCollection>();
            _genericTypes = new GenericConverterTypeCollection();
            _selectorGroups = new ConcurrentDictionary<Type, ConcurrentDictionary<string, ISelectorGroup>>();
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

        public void AddGeneric(Type openConverterType, Func<Type, Type> innerTypeSelector = null)
        {
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            _genericTypes.DefaultConverterType = new GenericConverterType(openConverterType, innerTypeSelector);
        }
        public void AddGeneric(Type openConverterType, Func<TypeInfo, PropertyInfo, bool> condition, Func<Type, Type> innerTypeSelector = null)
        {
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            _genericTypes.Conditionals.Add((condition, new GenericConverterType(openConverterType, innerTypeSelector)));
        }
        public void AddGeneric(Type openType, Type openConverterType, Func<Type, Type> innerTypeSelector = null)
        {
            if (openType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openType)} must be an open generic");
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            if (!_mappedGenericTypes.TryGetValue(openType, out var converters))
                _mappedGenericTypes.Add(openType, converters = new GenericConverterTypeCollection());
            converters.DefaultConverterType = new GenericConverterType(openConverterType, innerTypeSelector);
        }
        public void AddGeneric(Type openType, Type openConverterType, Func<TypeInfo, PropertyInfo, bool> condition, Func<Type, Type> innerTypeSelector = null)
        {
            if (openType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openType)} must be an open generic");
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            if (!_mappedGenericTypes.TryGetValue(openType, out var converters))
                _mappedGenericTypes.Add(openType, converters = new GenericConverterTypeCollection());
            converters.Conditionals.Add((condition, new GenericConverterType(openConverterType, innerTypeSelector)));
        }

        public void AddSelector(Serializer serializer, string groupKey, Type keyType, object keyValue, Type converterType)
        {
            var group = CreateSelectorGroup(keyType, groupKey);
            group.AddDynamicConverter(keyValue, BuildConverter(converterType, serializer));
        }

        public object Get(Serializer serializer, Type type, PropertyInfo propInfo = null, bool throwOnNotFound = true)
        {
            //Check parent
            object converter = _parent?.Get(serializer, type, propInfo, false);
            if (converter != null)
                return converter;

            if (propInfo == null) //Can only cache top-level due to attribute influences
            {
                if (_cache.TryGetValue(type, out var result))
                    return result;
                converter = Create(serializer, type, propInfo, throwOnNotFound);
                if (converter != null)
                    return _cache.GetOrAdd(type, converter);
                return null;
            }
            return Create(serializer, type, propInfo, throwOnNotFound);
        }
        private object Create(Serializer serializer, Type type, PropertyInfo propInfo, bool throwOnNotFound)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            //Mapped generic converters (List<T> -> CollectionPropertyConverter<T>)
            if (typeInfo.IsGenericType)
            {
                var openGenericType = typeInfo.GetGenericTypeDefinition();

                Type innerType = null;
                if (openGenericType == typeof(Dictionary<,>) || openGenericType == typeof(IReadOnlyDictionary<,>)) //TODO: We can only assume key type for JSON
                    innerType = typeInfo.GenericTypeArguments[1]; //TValue
                else if (openGenericType.GetTypeInfo().GenericTypeParameters.Length == 1)
                    innerType = typeInfo.GenericTypeArguments[0];

                if (innerType != null)
                {
                    var converterType = FindGenericConverterType(openGenericType, innerType, _mappedGenericTypes, typeInfo, propInfo);
                    if (converterType != null)
                    {
                        object innerConverter = serializer.GetConverter(innerType, propInfo);
                        return BuildConverter(converterType, serializer, innerConverter);
                    }
                }
            }

            //Normal converters (bool -> BooleanPropertyConverter)
            {
                var converterType = FindConverterType(type, _types, typeInfo, propInfo);
                if (converterType != null)
                    return BuildConverter(converterType, serializer);
            }
                        
            //Generic converters (Model -> ObjectPropertyConverter<Model>)
            {
                var converterType = FindGenericConverterType(type, _genericTypes, typeInfo, propInfo);
                if (converterType != null)
                {
                    if (type.IsArray) //We cant feed arrays through the mapped generic logic, emulate here
                    {
                        object innerConverter = serializer.GetConverter(type.GetElementType(), propInfo);
                        return BuildConverter(converterType, serializer, innerConverter);
                    }
                    else
                        return BuildConverter(converterType, serializer);
                }
            }

            if (throwOnNotFound)
                throw new InvalidOperationException($"Unsupported model type: {type.Name}");
            return null;
        }
        private object BuildConverter(Type converterType, Serializer serializer, object innerConverter = null)
        {
            var converterTypeInfo = converterType.GetTypeInfo();

            var constructors = converterTypeInfo.DeclaredConstructors.Where(x => !x.IsStatic).ToArray();
            if (constructors.Length == 0)
                throw new SerializationException($"{converterType.Name} is missing a constructor");
            if (constructors.Length != 1)
                throw new SerializationException($"{converterType.Name} has multiple constructors");
            var constructor = constructors[0];
            var parameters = constructor.GetParameters();

            var args = new object[parameters.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                if (i == args.Length - 1 && innerConverter != null)
                    args[i] = innerConverter;
                else if (_serializerType.IsAssignableFrom(paramType.GetTypeInfo()))
                    args[i] = serializer;
                else
                    throw new SerializationException($"{converterType.Name} has an unsupported constructor");
            }
            return constructor.Invoke(args);
        }

        private Type FindConverterType(Type type, Dictionary<Type, ConverterTypeCollection> collection, TypeInfo typeInfo, PropertyInfo propInfo)
        {
            if (collection.TryGetValue(type, out var converters))
                return FindConverterType(converters, typeInfo, propInfo);
            return null;
        }
        private Type FindGenericConverterType(Type type, Type innerType, Dictionary<Type, GenericConverterTypeCollection> collection, TypeInfo typeInfo, PropertyInfo propInfo)
        {
            if (collection.TryGetValue(type, out var converters))
                return FindGenericConverterType(innerType, converters, typeInfo, propInfo);
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
        private Type FindGenericConverterType(Type innerType, GenericConverterTypeCollection converters, TypeInfo typeInfo, PropertyInfo propInfo)
        {
            for (int i = 0; i < converters.Conditionals.Count; i++)
            {
                if (converters.Conditionals[i].Condition(typeInfo, propInfo))
                {
                    var converterType = converters.Conditionals[i].ConverterType;
                    return converterType.Type.MakeGenericType(converterType.InnerTypeSelector(innerType));
                }
            }
            if (converters.DefaultConverterType != null)
            {
                var converterType = converters.DefaultConverterType;
                return converterType.Type.MakeGenericType(converterType.InnerTypeSelector(innerType));
            }
            return null;
        }

        public ISelectorGroup GetSelectorGroup(Type keyType, string groupKey)
        {
            var selectorGroup = _parent?.GetSelectorGroup(keyType, groupKey);
            if (selectorGroup != null)
                return selectorGroup;

            if (_selectorGroups.TryGetValue(keyType, out var keyGroup) &&
                keyGroup.TryGetValue(groupKey, out selectorGroup))
                return selectorGroup;
            return null;
        }
        public ISelectorGroup CreateSelectorGroup(Type keyType, string groupKey)
        {
            var keyGroup = CreateSelectorKeyGroup(keyType);
            if (keyGroup.TryGetValue(groupKey, out var selectorGroup))
                return selectorGroup;
            return CreateSelectorGroup(keyType, keyGroup, groupKey);
        }
        private ISelectorGroup CreateSelectorGroup(Type keyType, ConcurrentDictionary<string, ISelectorGroup> keyGroup, string groupKey)
            => keyGroup.GetOrAdd(groupKey, Activator.CreateInstance(typeof(SelectorGroup<>).MakeGenericType(keyType)) as ISelectorGroup);
        private ConcurrentDictionary<string, ISelectorGroup> CreateSelectorKeyGroup(Type keyType)
            => _selectorGroups.GetOrAdd(keyType, _ => new ConcurrentDictionary<string, ISelectorGroup>());
    }
}
