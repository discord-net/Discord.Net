using System.Globalization;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class MessageTypeReader<T> : TypeReader
        where T : class, IMessage
    {
        public override async Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            ulong id;

            //By Id (1.0)
            if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
            {
                var msg = await context.Channel.GetMessageAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T;
                if (msg != null)
                    return TypeReaderResult.FromSuccess(msg);
            }

            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Message not found.");
        }
    }
}
