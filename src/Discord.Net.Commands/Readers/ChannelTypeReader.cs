using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
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
    public class ChannelTypeReader<T> : TypeReader
        where T : class, IChannel
    {
        /// <inheritdoc />
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (context.Guild != null)
            {
                var results = new Dictionary<ulong, TypeReaderValue>();
                var channels = await context.Guild.GetChannelsAsync(CacheMode.CacheOnly).ConfigureAwait(false);

                //By Mention (1.0)
                if (MentionUtils.TryParseChannel(input, out ulong id))
                    AddResult(results, await context.Guild.GetChannelAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T, 1.00f);

                //By Id (0.9)
                if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    AddResult(results, await context.Guild.GetChannelAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T, 0.90f);

                //By Name (0.7-0.8)
                foreach (var channel in channels.Where(x => string.Equals(input, x.Name, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, channel as T, channel.Name == input ? 0.80f : 0.70f);

                if (results.Count > 0)
                    return TypeReaderResult.FromSuccess(results.Values.ToReadOnlyCollection());
            }

            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Channel not found.");
        }

        private void AddResult(Dictionary<ulong, TypeReaderValue> results, T channel, float score)
        {
            if (channel != null && !results.ContainsKey(channel.Id))
                results.Add(channel.Id, new TypeReaderValue(channel, score));
        }
    }
}
