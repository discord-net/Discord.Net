using System.Globalization;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class MessageTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(IMessage context, string input)
        {
            //By Id
            ulong id;
            if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
            {
                var msg = context.Channel.GetCachedMessage(id);
                if (msg == null)
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Message not found."));
                else
                    return Task.FromResult(TypeReaderResult.FromSuccess(msg));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse Message Id."));
        }
    }
}
