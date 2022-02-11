using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Interactions
{
    internal class TypeMap<TConverter, TData>
        where TConverter : class, ITypeConverter<TData>
    {
        private readonly ConcurrentDictionary<Type, TConverter> _concretes;
        private readonly ConcurrentDictionary<Type, Type> _generics;
        private readonly InteractionService _interactionService;

        public TypeMap(InteractionService interactionService, IDictionary<Type, TConverter> concretes = null, IDictionary<Type, Type> generics = null)
        {
            _interactionService = interactionService;
            _concretes = concretes is not null ? new(concretes) : new();
            _generics = generics is not null ? new(generics) : new();
        }

        internal TConverter Get(Type type, IServiceProvider services = null)
        {
            if (_concretes.TryGetValue(type, out var specific))
                return specific;

            if (_generics.Any(x => x.Key.IsAssignableFrom(type)
                                   || x.Key.IsGenericTypeDefinition && type.IsGenericType && x.Key.GetGenericTypeDefinition() == type.GetGenericTypeDefinition()))
            {
                services ??= EmptyServiceProvider.Instance;

                var converterType = GetMostSpecific(type);
                var converter = ReflectionUtils<TConverter>.CreateObject(converterType.MakeGenericType(type).GetTypeInfo(), _interactionService, services);
                _concretes[type] = converter;
                return converter;
            }

            if (_concretes.Any(x => x.Value.CanConvertTo(type)))
                return _concretes.First(x => x.Value.CanConvertTo(type)).Value;

            throw new ArgumentException($"No type {typeof(TConverter).Name} is defined for this {type.FullName}", nameof(type));
        }

        public void AddConcrete<TTarget>(TConverter converter) =>
            AddConcrete(typeof(TTarget), converter);

        public void AddConcrete(Type type, TConverter converter)
        {
            if (!converter.CanConvertTo(type))
                throw new ArgumentException($"This {converter.GetType().FullName} cannot read {type.FullName} and cannot be registered as its {nameof(TypeConverter)}");

            _concretes[type] = converter;
        }

        public void AddGeneric<TTarget>(Type converterType) =>
            AddGeneric(typeof(TTarget), converterType);

        public void AddGeneric(Type targetType, Type converterType)
        {
            if (!converterType.IsGenericTypeDefinition)
                throw new ArgumentException($"{converterType.FullName} is not generic.");

            var genericArguments = converterType.GetGenericArguments();

            if (genericArguments.Length > 1)
                throw new InvalidOperationException($"Valid generic {converterType.FullName}s cannot have more than 1 generic type parameter");

            var constraints = genericArguments.SelectMany(x => x.GetGenericParameterConstraints());

            if (!constraints.Any(x => x.IsAssignableFrom(targetType)))
                throw new InvalidOperationException($"This generic class does not support type {targetType.FullName}");

            _generics[targetType] = converterType;
        }

        private Type GetMostSpecific(Type type)
        {
            if (_generics.TryGetValue(type, out var matching))
                return matching;

            if (type.IsGenericType && _generics.TryGetValue(type.GetGenericTypeDefinition(), out var genericDefinition))
                return genericDefinition;

            var typeInterfaces = type.GetInterfaces();
            var candidates = _generics.Where(x => x.Key.IsAssignableFrom(type))
                .OrderByDescending(x => typeInterfaces.Count(y => y.IsAssignableFrom(x.Key)));

            return candidates.First().Value;
        }
    }
}
