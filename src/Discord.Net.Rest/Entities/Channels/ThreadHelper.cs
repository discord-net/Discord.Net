using Discord.API;
using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            Action<ThreadChannelProperties> func,
            RequestOptions options)
        {
            var args = new ThreadChannelProperties();
            func(args);

            Preconditions.AtMost(args.AppliedTags.IsSpecified ? args.AppliedTags.Value.Count() : 0, 5, nameof(args.AppliedTags), "Forum post can have max 5 applied tags.");

            var apiArgs = new ModifyThreadParams
            {
                Name = args.Name,
                Archived = args.Archived,
                AutoArchiveDuration = args.AutoArchiveDuration,
                Locked = args.Locked,
                Slowmode = args.SlowModeInterval,
                AppliedTags = args.AppliedTags,
                Flags = args.Flags,
            };
            return await client.ApiClient.ModifyThreadAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
        }

        public static async Task<IReadOnlyCollection<RestThreadChannel>> GetActiveThreadsAsync(IGuild guild, ulong channelId, BaseDiscordClient client, RequestOptions options)
        {
            var result = await client.ApiClient.GetActiveThreadsAsync(guild.Id, options).ConfigureAwait(false);
            return result.Threads.Where(x => x.CategoryId == channelId).Select(x => RestThreadChannel.Create(client, guild, x)).ToImmutableArray();
        }

        public static async Task<IReadOnlyCollection<RestThreadChannel>> GetPublicArchivedThreadsAsync(IGuildChannel channel, BaseDiscordClient client, int? limit = null,
            DateTimeOffset? before = null, RequestOptions options = null)
        {
            var result = await client.ApiClient.GetPublicArchivedThreadsAsync(channel.Id, before, limit, options);
            return result.Threads.Select(x => RestThreadChannel.Create(client, channel.Guild, x)).ToImmutableArray();
        }

        public static async Task<IReadOnlyCollection<RestThreadChannel>> GetPrivateArchivedThreadsAsync(IGuildChannel channel, BaseDiscordClient client, int? limit = null,
            DateTimeOffset? before = null, RequestOptions options = null)
        {
            var result = await client.ApiClient.GetPrivateArchivedThreadsAsync(channel.Id, before, limit, options);
            return result.Threads.Select(x => RestThreadChannel.Create(client, channel.Guild, x)).ToImmutableArray();
        }

        public static async Task<IReadOnlyCollection<RestThreadChannel>> GetJoinedPrivateArchivedThreadsAsync(IGuildChannel channel, BaseDiscordClient client, int? limit = null,
            DateTimeOffset? before = null, RequestOptions options = null)
        {
            var result = await client.ApiClient.GetJoinedPrivateArchivedThreadsAsync(channel.Id, before, limit, options);
            return result.Threads.Select(x => RestThreadChannel.Create(client, channel.Guild, x)).ToImmutableArray();
        }

        public static IAsyncEnumerable<IReadOnlyCollection<RestThreadUser>> GetUsersAsync(IThreadChannel channel, BaseDiscordClient client, int limit = DiscordConfig.MaxThreadMembersPerBatch, ulong? afterId = null, RequestOptions options = null)
        {
            return new PagedAsyncEnumerable<RestThreadUser>(
                limit,
                async (info, ct) =>
                {
                    if (info.Position != null)
                        afterId = info.Position.Value;
                    var users = await client.ApiClient.ListThreadMembersAsync(channel.Id, afterId, limit, options);
                    return users.Select(x => RestThreadUser.Create(client, channel.Guild, x, channel)).ToImmutableArray();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != limit)
                        return false;
                    info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                start: afterId,
                count: limit
            );
        }

        public static async Task<RestThreadUser> GetUserAsync(ulong userId, IThreadChannel channel, BaseDiscordClient client, RequestOptions options = null)
        {
            var model = await client.ApiClient.GetThreadMemberAsync(channel.Id, userId, options).ConfigureAwait(false);

            return RestThreadUser.Create(client, channel.Guild, model, channel);
        }

        public static async Task<RestThreadChannel> CreatePostAsync(IForumChannel channel, BaseDiscordClient client, string title,
            ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string text = null, Embed embed = null,
            RequestOptions options = null, AllowedMentions allowedMentions = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, ulong[] tagIds = null)
        {
            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Length, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            Preconditions.AtMost(tagIds?.Length ?? 0, 5, nameof(tagIds), "Forum post can have max 5 applied tags.");

            // check that user flag and user Id list are exclusive, same with role flag and role Id list
            if (allowedMentions != null && allowedMentions.AllowedTypes.HasValue)
            {
                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Users) &&
                    allowedMentions.UserIds != null && allowedMentions.UserIds.Count > 0)
                {
                    throw new ArgumentException("The Users flag is mutually exclusive with the list of User Ids.", nameof(allowedMentions));
                }

                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Roles) &&
                    allowedMentions.RoleIds != null && allowedMentions.RoleIds.Count > 0)
                {
                    throw new ArgumentException("The Roles flag is mutually exclusive with the list of Role Ids.", nameof(allowedMentions));
                }
            }

            if (stickers != null)
            {
                Preconditions.AtMost(stickers.Length, 3, nameof(stickers), "A max of 3 stickers are allowed.");
            }

            if (flags is not MessageFlags.None and not MessageFlags.SuppressEmbeds)
                throw new ArgumentException("The only valid MessageFlags are SuppressEmbeds and none.", nameof(flags));

            if (channel.Flags.HasFlag(ChannelFlags.RequireTag))
                Preconditions.AtLeast(tagIds?.Length ?? 0, 1, nameof(tagIds), $"The channel {channel.Name} requires posts to have at least one tag.");

            var args = new CreatePostParams()
            {
                Title = title,
                ArchiveDuration = archiveDuration,
                Slowmode = slowmode,
                Message = new()
                {
                    AllowedMentions = allowedMentions.ToModel(),
                    Content = text,
                    Embeds = embeds.Any() ? embeds.Select(x => x.ToModel()).ToArray() : Optional<API.Embed[]>.Unspecified,
                    Flags = flags,
                    Components = components?.Components?.Any() ?? false ? components.Components.Select(x => new API.ActionRowComponent(x)).ToArray() : Optional<API.ActionRowComponent[]>.Unspecified,
                    Stickers = stickers?.Any() ?? false ? stickers.Select(x => x.Id).ToArray() : Optional<ulong[]>.Unspecified,
                },
                Tags = tagIds
            };

            var model = await client.ApiClient.CreatePostAsync(channel.Id, args, options).ConfigureAwait(false);

            return RestThreadChannel.Create(client, channel.Guild, model);
        }

        public static async Task<RestThreadChannel> CreatePostAsync(IForumChannel channel, BaseDiscordClient client, string title, IEnumerable<FileAttachment> attachments,
            ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageComponent components,
            ISticker[] stickers, Embed[] embeds, MessageFlags flags, ulong[] tagIds = null)
        {
            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Length, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            Preconditions.AtMost(tagIds?.Length ?? 0, 5, nameof(tagIds), "Forum post can have max 5 applied tags.");


            // check that user flag and user Id list are exclusive, same with role flag and role Id list
            if (allowedMentions != null && allowedMentions.AllowedTypes.HasValue)
            {
                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Users) &&
                    allowedMentions.UserIds != null && allowedMentions.UserIds.Count > 0)
                {
                    throw new ArgumentException("The Users flag is mutually exclusive with the list of User Ids.", nameof(allowedMentions));
                }

                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Roles) &&
                    allowedMentions.RoleIds != null && allowedMentions.RoleIds.Count > 0)
                {
                    throw new ArgumentException("The Roles flag is mutually exclusive with the list of Role Ids.", nameof(allowedMentions));
                }
            }

            if (stickers != null)
            {
                Preconditions.AtMost(stickers.Length, 3, nameof(stickers), "A max of 3 stickers are allowed.");
            }

            if (flags is not MessageFlags.None and not MessageFlags.SuppressEmbeds)
                throw new ArgumentException("The only valid MessageFlags are SuppressEmbeds and none.", nameof(flags));

            if (channel.Flags.HasFlag(ChannelFlags.RequireTag))
                throw new ArgumentException($"The channel {channel.Name} requires posts to have at least one tag.");

            var args = new CreateMultipartPostAsync(attachments.ToArray())
            {
                AllowedMentions = allowedMentions.ToModel(),
                ArchiveDuration = archiveDuration,
                Content = text,
                Embeds = embeds.Any() ? embeds.Select(x => x.ToModel()).ToArray() : Optional<API.Embed[]>.Unspecified,
                Flags = flags,
                MessageComponent = components?.Components?.Any() ?? false ? components.Components.Select(x => new API.ActionRowComponent(x)).ToArray() : Optional<API.ActionRowComponent[]>.Unspecified,
                Slowmode = slowmode,
                Stickers = stickers?.Any() ?? false ? stickers.Select(x => x.Id).ToArray() : Optional<ulong[]>.Unspecified,
                Title = title
            };

            var model = await client.ApiClient.CreatePostAsync(channel.Id, args, options);

            return RestThreadChannel.Create(client, channel.Guild, model);
        }
    }
}
