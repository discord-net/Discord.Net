using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class TypeReader
    {
        public abstract Task<TypeReaderResult> Read(ICommandContext context, string input);
    }
}
