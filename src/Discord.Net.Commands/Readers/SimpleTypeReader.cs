using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class SimpleTypeReader<T> : TypeReader
    {
        private readonly TryParseDelegate<T> _tryParse;

        public SimpleTypeReader()
        {
            _tryParse = PrimitiveParsers.Get<T>();
        }

        public override Task<TypeReaderResult> Read(IMessage context, string input)
        {
            T value;
            if (_tryParse(input, out value))
                return Task.FromResult(TypeReaderResult.FromSuccess(value));
            else
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {typeof(T).Name}"));
        }
    }
}
