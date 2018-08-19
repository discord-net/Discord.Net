using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public class MessageTypeReader<T> : TypeReader
        where T : class, IMessage
    {
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input,
            IServiceProvider services)
        {
            //By Id (1.0)
            if (!ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out var id))
                return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Message not found.");
            if (await context.Channel.GetMessageAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) is T msg)
                return TypeReaderResult.FromSuccess(msg);

            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Message not found.");
        }
    }
}
