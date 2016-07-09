using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class ChannelTypeReader<T> : TypeReader
        where T : class, IChannel
    {
        public override async Task<TypeReaderResult> Read(IMessage context, string input)
        {
            IGuildChannel guildChannel = context.Channel as IGuildChannel;
            IChannel result = null;

            if (guildChannel != null)
            {
                //By Id
                ulong id;
                if (MentionUtils.TryParseChannel(input, out id) || ulong.TryParse(input, out id))
                {
                    var channel = await guildChannel.Guild.GetChannelAsync(id).ConfigureAwait(false);
                    if (channel != null)
                        result = channel;
                }

                //By Name
                if (result == null)
                {
                    var channels = await guildChannel.Guild.GetChannelsAsync().ConfigureAwait(false);
                    var filteredChannels = channels.Where(x => string.Equals(input, x.Name, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (filteredChannels.Length > 1)
                        return TypeReaderResult.FromError(CommandError.MultipleMatches, "Multiple channels found.");
                    else if (filteredChannels.Length == 1)
                        result = filteredChannels[0];
                }
            }

            if (result == null)
                return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Channel not found.");

            T castResult = result as T;
            if (castResult == null)
                return TypeReaderResult.FromError(CommandError.CastFailed, $"Channel is not a {typeof(T).Name}.");
            else
                return TypeReaderResult.FromSuccess(castResult);
        }
    }
}
