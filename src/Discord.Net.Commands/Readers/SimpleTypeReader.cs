using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class SimpleTypeReader<T> : TypeReader
    {
        private readonly TryParseDelegate<T> _tryParse;

        private readonly float _score;

        public SimpleTypeReader(float score = 1)
        {
            _tryParse = PrimitiveParsers.Get<T>();
            _score = score;
        }

        public override Task<TypeReaderResult> Read(CommandContext context, string input)
        {
            T value;
            if (_tryParse(input, out value))
                return Task.FromResult(TypeReaderResult.FromSuccess(new TypeReaderValue(value, _score)));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {typeof(T).Name}"));
        }
    }
}
