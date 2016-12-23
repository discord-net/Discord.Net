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

        public PrimitiveTypeReader()
        {
            _tryParse = PrimitiveParsers.Get<T>();
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            T value;
            if (_tryParse(input, out value))
                return Task.FromResult(TypeReaderResult.FromSuccess(value));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {typeof(T).Name}"));
        }
    }
}
