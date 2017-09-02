using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class TypeReader
    {
        public abstract Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services);
    }
}
