using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Commands
{
    internal delegate bool TryParseDelegate<T>(string str, out T value);

    internal static class PrimitiveParsers
    {
        private static readonly Lazy<IReadOnlyDictionary<Type, Delegate>> _parsers = new Lazy<IReadOnlyDictionary<Type, Delegate>>(CreateParsers);

        public static IEnumerable<Type> SupportedTypes = _parsers.Value.Keys;

        static IReadOnlyDictionary<Type, Delegate> CreateParsers()
        {
            var parserBuilder = ImmutableDictionary.CreateBuilder<Type, Delegate>();
            parserBuilder[typeof(bool)] = (TryParseDelegate<bool>)bool.TryParse;
            parserBuilder[typeof(sbyte)] = (TryParseDelegate<sbyte>)sbyte.TryParse;
            parserBuilder[typeof(byte)] = (TryParseDelegate<byte>)byte.TryParse;
            parserBuilder[typeof(short)] = (TryParseDelegate<short>)short.TryParse;
            parserBuilder[typeof(ushort)] = (TryParseDelegate<ushort>)ushort.TryParse;
            parserBuilder[typeof(int)] = (TryParseDelegate<int>)int.TryParse;
            parserBuilder[typeof(uint)] = (TryParseDelegate<uint>)uint.TryParse;
            parserBuilder[typeof(long)] = (TryParseDelegate<long>)long.TryParse;
            parserBuilder[typeof(ulong)] = (TryParseDelegate<ulong>)ulong.TryParse;
            parserBuilder[typeof(float)] = (TryParseDelegate<float>)float.TryParse;
            parserBuilder[typeof(double)] = (TryParseDelegate<double>)double.TryParse;
            parserBuilder[typeof(decimal)] = (TryParseDelegate<decimal>)decimal.TryParse;
            parserBuilder[typeof(DateTime)] = (TryParseDelegate<DateTime>)DateTime.TryParse;
            parserBuilder[typeof(DateTimeOffset)] = (TryParseDelegate<DateTimeOffset>)DateTimeOffset.TryParse;
            parserBuilder[typeof(TimeSpan)] = (TryParseDelegate<TimeSpan>)TimeSpan.TryParse;
            parserBuilder[typeof(char)] = (TryParseDelegate<char>)char.TryParse;
            parserBuilder[typeof(string)] = (TryParseDelegate<string>)delegate (string str, out string value)
            {
                value = str;
                return true;
            };
            return parserBuilder.ToImmutable();
        }

        public static TryParseDelegate<T> Get<T>() => (TryParseDelegate<T>)_parsers.Value[typeof(T)];
        public static Delegate Get(Type type) => _parsers.Value[type];
    }
}
