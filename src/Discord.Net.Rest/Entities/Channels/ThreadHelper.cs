using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    internal static class ThreadHelper
    {
        public static async Task<Model> CreateThreadAsync(BaseDiscordClient client, ITextChannel channel, string name, ThreadType type = ThreadType.PublicThread,
            ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, RequestOptions options = null)
        {
            if (autoArchiveDuration == ThreadArchiveDuration.OneWeek && !channel.Guild.Features.Contains("SEVEN_DAY_THREAD_ARCHIVE"))
                throw new ArgumentException($"The guild {channel.Guild.Name} does not have the SEVEN_DAY_THREAD_ARCHIVE feature!");

            if (autoArchiveDuration == ThreadArchiveDuration.ThreeDays && !channel.Guild.Features.Contains("THREE_DAY_THREAD_ARCHIVE"))
                throw new ArgumentException($"The guild {channel.Guild.Name} does not have the THREE_DAY_THREAD_ARCHIVE feature!");

            if (type == ThreadType.PrivateThread && !channel.Guild.Features.Contains("PRIVATE_THREADS"))
                throw new ArgumentException($"The guild {channel.Guild.Name} does not have the PRIVATE_THREADS feature!");

            var args = new StartThreadParams()
            {
                Name = name,
                Duration = autoArchiveDuration,
                Type = type
            };

            Model model = null;

            if (message != null)
                model = await client.ApiClient.StartThreadAsync(channel.Id, message.Id, args, options).ConfigureAwait(false);
            else
                model = await client.ApiClient.StartThreadAsync(channel.Id, args, options).ConfigureAwait(false);

            return model;
        }

        public static async Task<Model> ModifyAsync(IThreadChannel channel, BaseDiscordClient client,
            Action<TextChannelProperties> func,
            RequestOptions options)
        {
            var args = new TextChannelProperties();
            func(args);
            var apiArgs = new API.Rest.ModifyThreadParams
            {
                Name = args.Name,
                Archived = args.Archived,
                AutoArchiveDuration = args.AutoArchiveDuration,
                Locked = args.Locked,
                Slowmode = args.SlowModeInterval
            };
            return await client.ApiClient.ModifyThreadAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
        }

        public static async Task<RestThreadUser[]> GetUsersAsync(IThreadChannel channel, BaseDiscordClient client, RequestOptions options = null)
        {
            var users = await client.ApiClient.ListThreadMembersAsync(channel.Id, options);

            return users.Select(x => RestThreadUser.Create(client, channel.Guild, x, channel)).ToArray();
        }

        public static async Task<RestThreadUser> GetUserAsync(ulong userdId, IThreadChannel channel, BaseDiscordClient client, RequestOptions options = null)
            => (await GetUsersAsync(channel, client, options).ConfigureAwait(false)).FirstOrDefault(x => x.Id == userdId);
    }
}
