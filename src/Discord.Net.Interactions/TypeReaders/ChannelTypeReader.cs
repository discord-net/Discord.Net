using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     A <see cref="TypeReader"/> for parsing objects implementing <see cref="IChannel"/>.
    /// </summary>
    /// <remarks>
    ///     This <see cref="TypeReader"/> is shipped with Discord.Net and is used by default to parse any 
    ///     <see cref="IChannel"/> implemented object within a command. The TypeReader will attempt to first parse the
    ///     input by mention, then the snowflake identifier, then by name; the highest candidate will be chosen as the
    ///     final output; otherwise, an erroneous <see cref="TypeReaderResult"/> is returned.
    /// </remarks>
    /// <typeparam name="T">The type to be checked; must implement <see cref="IChannel"/>.</typeparam>
    internal class ChannelTypeReader<T> : TypeReader<T> where T : class, IChannel
    {
        /// <inheritdoc />
        public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, object input, IServiceProvider services)
        {
            if (context.Guild is not null)
            {
                var str = input as string;

                if (ulong.TryParse(str, out var channelId))
                {
                    var channel = await context.Guild.GetChannelAsync(channelId).ConfigureAwait(false);

                    if(channel is not null)
                        return TypeConverterResult.FromSuccess(channel as T);
                }

                if (MentionUtils.TryParseChannel(str, out channelId))
                {
                    var channel = await context.Guild.GetChannelAsync(channelId).ConfigureAwait(false);

                    if(channel is not null)
                        return TypeConverterResult.FromSuccess(channel as T);
                }

                var channels = await context.Guild.GetChannelsAsync().ConfigureAwait(false);
                var nameMatch = channels.FirstOrDefault(x => string.Equals(x.Name, str, StringComparison.OrdinalIgnoreCase));

                if (nameMatch is not null)
                    return TypeConverterResult.FromSuccess(nameMatch as T);
            }

            return TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "Channel not found.");
        }

        public override string Serialize(object value) => (value as IChannel)?.Id.ToString();
    }
}
