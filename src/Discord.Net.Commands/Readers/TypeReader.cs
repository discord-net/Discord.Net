using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class TypeReader
    {
        public abstract Task<TypeReaderResult> Read(CommandContext context, string input);
    }
}
