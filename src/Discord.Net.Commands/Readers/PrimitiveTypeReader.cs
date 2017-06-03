using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal static class PrimitiveTypeReader
    {
        public static TypeReader Create(Type type)
        {
            type = typeof(PrimitiveTypeReader<>).MakeGenericType(type);
            return Activator.CreateInstance(type) as TypeReader;
        }
    }

    internal class PrimitiveTypeReader<T> : TypeReader
    {
        private readonly TryParseDelegate<T> _tryParse;
        private readonly float _score;

        public PrimitiveTypeReader()
            : this(1, PrimitiveParsers.Get<T>())
        { }

        public PrimitiveTypeReader(float score, TryParseDelegate<T> tryParse)
        {
            _tryParse = tryParse;
            _score = score;
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            T value;
            if (_tryParse(input, out value))
                return Task.FromResult(TypeReaderResult.FromSuccess(new TypeReaderValue(value, _score)));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {typeof(T).Name}"));
        }
    }
}
