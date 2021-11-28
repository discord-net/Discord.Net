using Discord.API.Rest;
using System;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    internal static class ThreadHelper
    {
        public static async Task<Model> CreateThreadAsync(BaseDiscordClient client, ITextChannel channel, string name, ThreadType type = ThreadType.PublicThread,
            ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
        {
            var features = channel.Guild.Features;
            if (autoArchiveDuration == ThreadArchiveDuration.OneWeek && !features.HasFeature(GuildFeature.SevenDayThreadArchive))
                throw new ArgumentException($"The guild {channel.Guild.Name} does not have the SEVEN_DAY_THREAD_ARCHIVE feature!", nameof(autoArchiveDuration));

            if (autoArchiveDuration == ThreadArchiveDuration.ThreeDays && !features.HasFeature(GuildFeature.ThreeDayThreadArchive))
                throw new ArgumentException($"The guild {channel.Guild.Name} does not have the THREE_DAY_THREAD_ARCHIVE feature!", nameof(autoArchiveDuration));

            if (type == ThreadType.PrivateThread && !features.HasFeature(GuildFeature.PrivateThreads))
                throw new ArgumentException($"The guild {channel.Guild.Name} does not have the PRIVATE_THREADS feature!", nameof(type));

            if (channel is INewsChannel && type != ThreadType.NewsThread)
                throw new ArgumentException($"{nameof(type)} must be a {ThreadType.NewsThread} in News channels");

            var args = new StartThreadParams
            {
                Name = name,
                Duration = autoArchiveDuration,
                Type = type,
                Invitable = invitable.HasValue ? invitable.Value : Optional<bool>.Unspecified,
                Ratelimit = slowmode.HasValue ? slowmode.Value : Optional<int?>.Unspecified,
            };

            Model model;

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
            var apiArgs = new ModifyThreadParams
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

        public static async Task<RestThreadUser> GetUserAsync(ulong userId, IThreadChannel channel, BaseDiscordClient client, RequestOptions options = null)
        {
            var model = await client.ApiClient.GetThreadMemberAsync(channel.Id, userId, options).ConfigureAwait(false);

            return RestThreadUser.Create(client, channel.Guild, model, channel);
        }
    }
}
