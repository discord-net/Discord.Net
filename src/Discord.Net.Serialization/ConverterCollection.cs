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
            public List<(Func<PropertyInfo, bool> Condition, Type ConverterType)> Conditionals = new List<(Func<PropertyInfo, bool>, Type)>();
        }

        private static readonly MethodInfo _getConverterMethod
            = typeof(ConverterCollection).GetTypeInfo().GetDeclaredMethod(nameof(Get));

        private readonly ConcurrentDictionary<Type, object> _maps = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, ConverterTypeCollection> _types = new ConcurrentDictionary<Type, ConverterTypeCollection>();
        private readonly ConcurrentDictionary<Type, ConverterTypeCollection> _genericTypes = new ConcurrentDictionary<Type, ConverterTypeCollection>();

        internal ConverterCollection() { }

        public void Add<TType, TConverter>()
        {
            var converters = _types.GetOrAdd(typeof(TType), _ => new ConverterTypeCollection());
            converters.DefaultConverterType = typeof(TConverter);
        }
        public void Add<TType, TConverter>(Func<PropertyInfo, bool> condition)
        {
            var converters = _types.GetOrAdd(typeof(TType), _ => new ConverterTypeCollection());
            converters.Conditionals.Add((condition, typeof(TConverter)));
        }

        public void AddGeneric(Type openType, Type openConverterType)
        {
            if (openType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openType)} must be an open generic");
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            var converters = _genericTypes.GetOrAdd(openType, _ => new ConverterTypeCollection());
            converters.DefaultConverterType = openConverterType;
        }
        public void AddGeneric(Type openType, Type openConverterType, Func<PropertyInfo, bool> condition)
        {
            if (openType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openType)} must be an open generic");
            if (openConverterType.IsConstructedGenericType) throw new InvalidOperationException($"{nameof(openConverterType)} must be an open generic");
            var converters = _genericTypes.GetOrAdd(openType, _ => new ConverterTypeCollection());
            converters.Conditionals.Add((condition, openConverterType));
        }
        
        public object Get<TType>(PropertyInfo propInfo)
        {
            var typeInfo = typeof(TType).GetTypeInfo();

            //Generic converters
            if (typeInfo.IsGenericType)
            {
                var converterType = FindConverterType(typeInfo.GetGenericTypeDefinition(), _genericTypes, propInfo);
                if (converterType != null)
                {
                    var innerType = typeInfo.GenericTypeArguments[0];
                    converterType = converterType.MakeGenericType(innerType);
                    object innerConverter = GetInnerConverter(innerType, propInfo);
                    return Activator.CreateInstance(converterType, innerConverter);
                }
            }

            //Normal converters
            {
                var converterType = FindConverterType(typeof(TType), _types, propInfo);
                if (converterType != null)
                    return Activator.CreateInstance(converterType);
            }

            throw new InvalidOperationException($"Unsupported model type: {typeof(TType).Name}");
        }
        private object GetInnerConverter(Type type, PropertyInfo propInfo)
            => _getConverterMethod.MakeGenericMethod(type).Invoke(this, new object[] { propInfo });

        private Type FindConverterType(Type type, ConcurrentDictionary<Type, ConverterTypeCollection> collection, PropertyInfo propInfo)
        {
            if (collection.TryGetValue(type, out var converters))
            {
                for (int i = 0; i < converters.Conditionals.Count; i++)
                {
                    if (converters.Conditionals[i].Condition(propInfo))
                        return converters.Conditionals[i].ConverterType;
                }
                if (converters.DefaultConverterType != null)
                    return converters.DefaultConverterType;
            }
            return null;
        }
    }
}
