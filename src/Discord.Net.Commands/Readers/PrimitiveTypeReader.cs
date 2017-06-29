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
            : this(PrimitiveParsers.Get<T>(), 1)
        { }

        public PrimitiveTypeReader(TryParseDelegate<T> tryParse, float score)
        {
            if (score < 0 || score > 1)
                throw new ArgumentOutOfRangeException(nameof(score), score, "Scores must be within the range [0, 1]");

            _tryParse = tryParse;
            _score = score;
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            if (_tryParse(input, out T value))
                return Task.FromResult(TypeReaderResult.FromSuccess(new TypeReaderValue(value, _score)));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {typeof(T).Name}"));
        }
    }
}
