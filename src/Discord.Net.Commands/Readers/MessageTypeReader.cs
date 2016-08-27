using System.Globalization;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class MessageTypeReader<T> : TypeReader
        where T : class, IMessage
    {
        public override Task<TypeReaderResult> Read(IUserMessage context, string input)
        {
            ulong id;

            //By Id (1.0)
            if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
            {
                var msg = context.Channel.GetCachedMessage(id) as T;
                if (msg != null)
                    return Task.FromResult(TypeReaderResult.FromSuccess(msg));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Message not found."));
        }
    }
}
