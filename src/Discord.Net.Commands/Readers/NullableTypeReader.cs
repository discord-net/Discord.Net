using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal static class NullableTypeReader
    {
        public static TypeReader Create(Type type)
        {
            type = typeof(NullableTypeReader<>).MakeGenericType(type);
            return Activator.CreateInstance(type) as TypeReader;
        }
    }

    internal class NullableTypeReader<T> : TypeReader
        where T : struct
    {
        private readonly TryParseDelegate<T> _tryParse;

        public NullableTypeReader()
        {
            _tryParse = PrimitiveParsers.Get<T>();
        }

        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            T value;
            if (_tryParse(input, out value))
                return Task.FromResult(TypeReaderResult.FromSuccess(new Nullable<T>(value)));
            return Task.FromResult(TypeReaderResult.FromSuccess(new Nullable<T>()));
        }
    }
}
