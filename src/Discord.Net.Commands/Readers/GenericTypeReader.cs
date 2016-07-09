using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class GenericTypeReader : TypeReader
    {
        private readonly Func<IMessage, string, Task<TypeReaderResult>> _action;

        public GenericTypeReader(Func<IMessage, string, Task<TypeReaderResult>> action)
        {
            _action = action;
        }

        public override Task<TypeReaderResult> Read(IMessage context, string input) => _action(context, input);
    }
}
