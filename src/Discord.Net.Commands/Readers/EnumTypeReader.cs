using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Commands
{
    delegate bool TryParseDelegate<T>(string str, out T value);

    internal static class EnumTypeReader
    {        
        private static readonly IReadOnlyDictionary<Type, object> _parsers;

        static EnumTypeReader()
        {
            var parserBuilder = ImmutableDictionary.CreateBuilder<Type, object>();
            parserBuilder[typeof(sbyte)] = (TryParseDelegate<sbyte>)sbyte.TryParse;
            parserBuilder[typeof(byte)] = (TryParseDelegate<byte>)byte.TryParse;
            parserBuilder[typeof(short)] = (TryParseDelegate<short>)short.TryParse;
            parserBuilder[typeof(ushort)] = (TryParseDelegate<ushort>)ushort.TryParse;
            parserBuilder[typeof(int)] = (TryParseDelegate<int>)int.TryParse;
            parserBuilder[typeof(uint)] = (TryParseDelegate<uint>)uint.TryParse;
            parserBuilder[typeof(long)] = (TryParseDelegate<long>)long.TryParse;
            parserBuilder[typeof(ulong)] = (TryParseDelegate<ulong>)ulong.TryParse;
            _parsers = parserBuilder.ToImmutable();
        }

        public static TypeReader GetReader(Type type)
        {
            Type baseType = Enum.GetUnderlyingType(type);
            var constructor = typeof(EnumTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
            return (TypeReader)constructor.Invoke(new object[] { type, _parsers[baseType] });
        }
    }

    internal class EnumTypeReader<T> : TypeReader
    {
        private readonly IReadOnlyDictionary<string, object> _enumsByName;
        private readonly IReadOnlyDictionary<T, object> _enumsByValue;
        private readonly Type _enumType;
        private readonly TryParseDelegate<T> _tryParse;
        
        public EnumTypeReader(Type type, TryParseDelegate<T> parser)
        {
            _enumType = type;
            _tryParse = parser;

            var byNameBuilder = ImmutableDictionary.CreateBuilder<string, object>();
            var byValueBuilder = ImmutableDictionary.CreateBuilder<T, object>();
            
            foreach (var v in Enum.GetValues(_enumType))
            {
                byNameBuilder.Add(v.ToString().ToLower(), v);
                byValueBuilder.Add((T)v, v);
            }

            _enumsByName = byNameBuilder.ToImmutable();
            _enumsByValue = byValueBuilder.ToImmutable();
        }

        public override Task<TypeReaderResult> Read(IMessage context, string input)
        {
            T baseValue;
            object enumValue;

            if (_tryParse(input, out baseValue))
            {
                if (_enumsByValue.TryGetValue(baseValue, out enumValue))
                    return Task.FromResult(TypeReaderResult.FromSuccess(enumValue));
                else
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.CastFailed, $"Value is not a {_enumType.Name}"));
            }
            else
            {
                if (_enumsByName.TryGetValue(input.ToLower(), out enumValue))
                    return Task.FromResult(TypeReaderResult.FromSuccess(enumValue));
                else
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.CastFailed, $"Value is not a {_enumType.Name}"));
            }
        }
    }
}
