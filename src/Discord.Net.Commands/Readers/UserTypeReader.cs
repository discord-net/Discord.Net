using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     A <see cref="TypeReader"/> for parsing objects implementing <see cref="IUser"/>.
    /// </summary>
    /// <typeparam name="T">The type to be checked; must implement <see cref="IUser"/>.</typeparam>
    public class UserTypeReader<T> : TypeReader
        where T : class, IUser
    {
        /// <inheritdoc />
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var results = new Dictionary<ulong, TypeReaderValue>();
            IAsyncEnumerable<IUser> channelUsers = context.Channel.GetUsersAsync(CacheMode.CacheOnly).Flatten(); // it's better
            IReadOnlyCollection<IGuildUser> guildUsers = ImmutableArray.Create<IGuildUser>();

            if (context.Guild != null)
                guildUsers = await context.Guild.GetUsersAsync(CacheMode.CacheOnly).ConfigureAwait(false);

            //By Mention (1.0)
            if (MentionUtils.TryParseUser(input, out var id))
            {
                if (context.Guild != null)
                    AddResult(results, await context.Guild.GetUserAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T, 1.00f);
                else
                    AddResult(results, await context.Channel.GetUserAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T, 1.00f);
            }

            //By Id (0.9)
            if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
            {
                if (context.Guild != null)
                    AddResult(results, await context.Guild.GetUserAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T, 0.90f);
                else
                    AddResult(results, await context.Channel.GetUserAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T, 0.90f);
            }

            //By Username + Discriminator (0.7-0.85)
            int index = input.LastIndexOf('#');
            if (index >= 0)
            {
                string username = input.Substring(0, index);
                if (ushort.TryParse(input.Substring(index + 1), out ushort discriminator))
                {
                    var channelUser = await channelUsers.FirstOrDefaultAsync(x => x.DiscriminatorValue == discriminator &&
                        string.Equals(username, x.Username, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);
                    AddResult(results, channelUser as T, channelUser?.Username == username ? 0.85f : 0.75f);

                    var guildUser = guildUsers.FirstOrDefault(x => x.DiscriminatorValue == discriminator &&
                        string.Equals(username, x.Username, StringComparison.OrdinalIgnoreCase));
                    AddResult(results, guildUser as T, guildUser?.Username == username ? 0.80f : 0.70f);
                }
            }

            //By Username (0.5-0.6)
            {
                await channelUsers
                    .Where(x => string.Equals(input, x.Username, StringComparison.OrdinalIgnoreCase))
                    .ForEachAsync(channelUser => AddResult(results, channelUser as T, channelUser.Username == input ? 0.65f : 0.55f))
                    .ConfigureAwait(false);
                
                foreach (var guildUser in guildUsers.Where(x => string.Equals(input, x.Username, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, guildUser as T, guildUser.Username == input ? 0.60f : 0.50f);
            }

            //By Nickname (0.5-0.6)
            {
                await channelUsers
                    .Where(x => string.Equals(input, (x as IGuildUser)?.Nickname, StringComparison.OrdinalIgnoreCase))
                    .ForEachAsync(channelUser => AddResult(results, channelUser as T, (channelUser as IGuildUser).Nickname == input ? 0.65f : 0.55f))
                    .ConfigureAwait(false);

                foreach (var guildUser in guildUsers.Where(x => string.Equals(input, x.Nickname, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, guildUser as T, guildUser.Nickname == input ? 0.60f : 0.50f);
            }

            if (results.Count > 0)
                return TypeReaderResult.FromSuccess(results.Values.ToImmutableArray());
            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "User not found.");
        }

        private void AddResult(Dictionary<ulong, TypeReaderValue> results, T user, float score)
        {
            if (user != null && !results.ContainsKey(user.Id))
                results.Add(user.Id, new TypeReaderValue(user, score));
        }
    }
}
