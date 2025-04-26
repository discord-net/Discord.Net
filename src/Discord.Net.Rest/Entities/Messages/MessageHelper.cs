using Discord.API;
using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Model = Discord.API.Message;
using UserModel = Discord.API.User;

namespace Discord.Rest
{
    internal static class MessageHelper
    {
        /// <summary>
        /// Regex used to check if some text is formatted as inline code.
        /// </summary>
        private static readonly Regex InlineCodeRegex = new Regex(@"[^\\]?(`).+?[^\\](`)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

        /// <summary>
        /// Regex used to check if some text is formatted as a code block.
        /// </summary>
        private static readonly Regex BlockCodeRegex = new Regex(@"[^\\]?(```).+?[^\\](```)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

        /// <exception cref="InvalidOperationException">Only the author of a message may modify the message.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public static Task<Model> ModifyAsync(IMessage msg, BaseDiscordClient client, Action<MessageProperties> func,
            RequestOptions options)
            => ModifyAsync(msg.Channel.Id, msg.Id, client, func, options);

        public static Task<Model> ModifyAsync(ulong channelId, ulong msgId, BaseDiscordClient client, Action<MessageProperties> func,
            RequestOptions options)
        {
            var args = new MessageProperties();
            func(args);

            var embed = args.Embed;
            var embeds = args.Embeds;

            bool hasText = args.Content.IsSpecified && string.IsNullOrEmpty(args.Content.Value);
            bool hasEmbeds = embed.IsSpecified && embed.Value != null || embeds.IsSpecified && embeds.Value?.Length > 0;
            bool hasComponents = args.Components.IsSpecified && args.Components.Value != null;
            bool hasAttachments = args.Attachments.IsSpecified;
            bool hasFlags = args.Flags.IsSpecified;

            // No content needed if modifying flags
            if ((!hasComponents && !hasText && !hasEmbeds && !hasAttachments) && !hasFlags)
                Preconditions.NotNullOrEmpty(args.Content.IsSpecified ? args.Content.Value : string.Empty, nameof(args.Content));

            if (args.AllowedMentions.IsSpecified)
            {
                AllowedMentions allowedMentions = args.AllowedMentions.Value;
                Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
                Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");

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
            }

            var apiEmbeds = embed.IsSpecified || embeds.IsSpecified ? new List<API.Embed>() : null;

            if (embed.IsSpecified && embed.Value != null)
            {
                apiEmbeds.Add(embed.Value.ToModel());
            }

            if (embeds.IsSpecified && embeds.Value != null)
            {
                apiEmbeds.AddRange(embeds.Value.Select(x => x.ToModel()));
            }

            Preconditions.AtMost(apiEmbeds?.Count ?? 0, 10, nameof(args.Embeds), "A max of 10 embeds are allowed.");

            if (!args.Attachments.IsSpecified)
            {
                var apiArgs = new API.Rest.ModifyMessageParams
                {
                    Content = args.Content,
                    Embeds = apiEmbeds?.ToArray() ?? Optional<API.Embed[]>.Unspecified,
                    Flags = args.Flags.IsSpecified ? args.Flags.Value : Optional.Create<MessageFlags?>(),
                    AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value.ToModel() : Optional.Create<API.AllowedMentions>(),
                    Components = args.Components.IsSpecified ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Array.Empty<API.ActionRowComponent>() : Optional<API.ActionRowComponent[]>.Unspecified,
                };
                return client.ApiClient.ModifyMessageAsync(channelId, msgId, apiArgs, options);
            }
            else
            {
                var attachments = args.Attachments.Value?.ToArray() ?? Array.Empty<FileAttachment>();

                var apiArgs = new UploadFileParams(attachments)
                {
                    Content = args.Content,
                    Embeds = apiEmbeds?.ToArray() ?? Optional<API.Embed[]>.Unspecified,
                    Flags = args.Flags.IsSpecified ? args.Flags.Value : Optional.Create<MessageFlags?>(),
                    AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value.ToModel() : Optional.Create<API.AllowedMentions>(),
                    MessageComponent = args.Components.IsSpecified ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Array.Empty<API.ActionRowComponent>() : Optional<API.ActionRowComponent[]>.Unspecified
                };

                return client.ApiClient.ModifyMessageAsync(channelId, msgId, apiArgs, options);
            }
        }

        public static Task DeleteAsync(IMessage msg, BaseDiscordClient client, RequestOptions options)
            => DeleteAsync(msg.Channel.Id, msg.Id, client, options);

        public static Task DeleteAsync(ulong channelId, ulong msgId, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.DeleteMessageAsync(channelId, msgId, options);

        public static Task AddReactionAsync(ulong channelId, ulong messageId, IEmote emote, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.AddReactionAsync(channelId, messageId, emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name), options);

        public static Task AddReactionAsync(IMessage msg, IEmote emote, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.AddReactionAsync(msg.Channel.Id, msg.Id, emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name), options);

        public static Task RemoveReactionAsync(ulong channelId, ulong messageId, ulong userId, IEmote emote, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.RemoveReactionAsync(channelId, messageId, userId, emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name), options);

        public static Task RemoveReactionAsync(IMessage msg, ulong userId, IEmote emote, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.RemoveReactionAsync(msg.Channel.Id, msg.Id, userId, emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name), options);

        public static Task RemoveAllReactionsAsync(ulong channelId, ulong messageId, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.RemoveAllReactionsAsync(channelId, messageId, options);

        public static Task RemoveAllReactionsAsync(IMessage msg, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.RemoveAllReactionsAsync(msg.Channel.Id, msg.Id, options);

        public static Task RemoveAllReactionsForEmoteAsync(ulong channelId, ulong messageId, IEmote emote, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.RemoveAllReactionsForEmoteAsync(channelId, messageId, emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name), options);

        public static Task RemoveAllReactionsForEmoteAsync(IMessage msg, IEmote emote, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.RemoveAllReactionsForEmoteAsync(msg.Channel.Id, msg.Id, emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name), options);

        public static IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IMessage msg, IEmote emote,
            int? limit, BaseDiscordClient client, ReactionType reactionType, RequestOptions options)
        {
            Preconditions.NotNull(emote, nameof(emote));
            var emoji = (emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name));

            return new PagedAsyncEnumerable<IUser>(
                DiscordConfig.MaxUserReactionsPerBatch,
                async (info, ct) =>
                {
                    var args = new GetReactionUsersParams
                    {
                        Limit = info.PageSize
                    };

                    if (info.Position != null)
                        args.AfterUserId = info.Position.Value;

                    var models = await client.ApiClient.GetReactionUsersAsync(msg.Channel.Id, msg.Id, emoji, args, reactionType, options).ConfigureAwait(false);
                    return models.Select(x => RestUser.Create(client, x)).ToImmutableArray();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxUserReactionsPerBatch)
                        return false;

                    info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                count: limit
            );
        }

        private static string UrlEncode(string text)
        {
#if NET461
            return System.Net.WebUtility.UrlEncode(text);
#else
            return System.Web.HttpUtility.UrlEncode(text);
#endif
        }
        public static string SanitizeMessage(IMessage message)
        {
            var newContent = MentionUtils.Resolve(message, 0, TagHandling.FullName, TagHandling.FullName, TagHandling.FullName, TagHandling.FullName, TagHandling.FullName);
            newContent = Format.StripMarkDown(newContent);
            return newContent;
        }

        public static Task PinAsync(IMessage msg, BaseDiscordClient client, RequestOptions options)
        {
            if (msg.Channel is IVoiceChannel)
                throw new NotSupportedException("Pinned messages are not supported in text-in-voice channels.");
            return client.ApiClient.AddPinAsync(msg.Channel.Id, msg.Id, options);
        }

        public static Task UnpinAsync(IMessage msg, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.RemovePinAsync(msg.Channel.Id, msg.Id, options);

        public static ImmutableArray<ITag> ParseTags(string text, IMessageChannel channel, IGuild guild, IReadOnlyCollection<IUser> userMentions)
        {
            var tags = ImmutableArray.CreateBuilder<ITag>();
            int index = 0;
            var codeIndex = 0;

            // checks if the tag being parsed is wrapped in code blocks
            bool CheckWrappedCode()
            {
                // util to check if the index of a tag is within the bounds of the codeblock
                bool EnclosedInBlock(Match m)
                    => m.Groups[1].Index < index && index < m.Groups[2].Index;

                // loop through all code blocks that are before the start of the tag
                while (codeIndex < index)
                {
                    var blockMatch = BlockCodeRegex.Match(text, codeIndex);
                    if (blockMatch.Success)
                    {
                        if (EnclosedInBlock(blockMatch))
                            return true;
                        // continue if the end of the current code was before the start of the tag
                        codeIndex += blockMatch.Groups[2].Index + blockMatch.Groups[2].Length;
                        if (codeIndex < index)
                            continue;
                        return false;
                    }
                    var inlineMatch = InlineCodeRegex.Match(text, codeIndex);
                    if (inlineMatch.Success)
                    {
                        if (EnclosedInBlock(inlineMatch))
                            return true;
                        // continue if the end of the current code was before the start of the tag
                        codeIndex += inlineMatch.Groups[2].Index + inlineMatch.Groups[2].Length;
                        if (codeIndex < index)
                            continue;
                        return false;
                    }
                    return false;
                }
                return false;
            }

            while (true)
            {
                index = text.IndexOf('<', index);
                if (index == -1)
                    break;
                int endIndex = text.IndexOf('>', index + 1);
                if (endIndex == -1)
                    break;
                if (CheckWrappedCode())
                    break;
                string content = text.Substring(index, endIndex - index + 1);

                if (MentionUtils.TryParseUser(content, out ulong id))
                {
                    IUser mentionedUser = null;
                    foreach (var mention in userMentions)
                    {
                        if (mention.Id == id)
                        {
                            mentionedUser = channel?.GetUserAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult();
                            if (mentionedUser == null)
                                mentionedUser = mention;
                            break;
                        }
                    }
                    tags.Add(new Tag<IUser>(TagType.UserMention, index, content.Length, id, mentionedUser));
                }
                else if (MentionUtils.TryParseChannel(content, out id))
                {
                    IChannel mentionedChannel = null;
                    if (guild != null)
                        mentionedChannel = guild.GetChannelAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult();
                    tags.Add(new Tag<IChannel>(TagType.ChannelMention, index, content.Length, id, mentionedChannel));
                }
                else if (MentionUtils.TryParseRole(content, out id))
                {
                    IRole mentionedRole = null;
                    if (guild != null)
                        mentionedRole = guild.GetRole(id);
                    tags.Add(new Tag<IRole>(TagType.RoleMention, index, content.Length, id, mentionedRole));
                }
                else if (Emote.TryParse(content, out var emoji))
                    tags.Add(new Tag<Emote>(TagType.Emoji, index, content.Length, emoji.Id, emoji));
                else //Bad Tag
                {
                    index++;
                    continue;
                }
                index = endIndex + 1;
            }

            index = 0;
            codeIndex = 0;
            while (true)
            {
                index = text.IndexOf("@everyone", index);
                if (index == -1)
                    break;
                if (CheckWrappedCode())
                    break;
                var tagIndex = FindIndex(tags, index);
                if (tagIndex.HasValue)
                    tags.Insert(tagIndex.Value, new Tag<IRole>(TagType.EveryoneMention, index, "@everyone".Length, 0, guild?.EveryoneRole));
                index++;
            }

            index = 0;
            codeIndex = 0;
            while (true)
            {
                index = text.IndexOf("@here", index);
                if (index == -1)
                    break;
                if (CheckWrappedCode())
                    break;
                var tagIndex = FindIndex(tags, index);
                if (tagIndex.HasValue)
                    tags.Insert(tagIndex.Value, new Tag<IRole>(TagType.HereMention, index, "@here".Length, 0, guild?.EveryoneRole));
                index++;
            }

            return tags.ToImmutable();
        }

        private static int? FindIndex(IReadOnlyList<ITag> tags, int index)
        {
            int i = 0;
            for (; i < tags.Count; i++)
            {
                var tag = tags[i];
                if (index < tag.Index)
                    break; //Position before this tag
            }
            if (i > 0 && index < tags[i - 1].Index + tags[i - 1].Length)
                return null; //Overlaps tag before this
            return i;
        }

        public static ImmutableArray<ulong> FilterTagsByKey(TagType type, ImmutableArray<ITag> tags)
        {
            return tags
                .Where(x => x.Type == type)
                .Select(x => x.Key)
                .ToImmutableArray();
        }

        public static ImmutableArray<T> FilterTagsByValue<T>(TagType type, ImmutableArray<ITag> tags)
        {
            return tags
                .Where(x => x.Type == type)
                .Select(x => (T)x.Value)
                .Where(x => x != null)
                .ToImmutableArray();
        }

        public static MessageSource GetSource(Model msg)
        {
            if (msg.Type != MessageType.Default && msg.Type != MessageType.Reply)
                return MessageSource.System;
            else if (msg.WebhookId.IsSpecified)
                return MessageSource.Webhook;
            else if (msg.Author.GetValueOrDefault()?.Bot.GetValueOrDefault(false) == true)
                return MessageSource.Bot;
            return MessageSource.User;
        }

        public static Task CrosspostAsync(IMessage msg, BaseDiscordClient client, RequestOptions options)
            => CrosspostAsync(msg.Channel.Id, msg.Id, client, options);

        public static Task CrosspostAsync(ulong channelId, ulong msgId, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.CrosspostAsync(channelId, msgId, options);

        public static IUser GetAuthor(BaseDiscordClient client, IGuild guild, UserModel model, ulong? webhookId)
        {
            IUser author = null;
            if (guild != null)
                author = guild.GetUserAsync(model.Id, CacheMode.CacheOnly).Result;
            if (author == null)
                author = RestUser.Create(client, guild, model, webhookId);
            return author;
        }

        public static IAsyncEnumerable<IReadOnlyCollection<IUser>> GetPollAnswerVotersAsync(ulong channelId, ulong msgId, ulong? afterId,
            uint answerId, int? limit, BaseDiscordClient client, RequestOptions options)
        {
            return new PagedAsyncEnumerable<IUser>(
                DiscordConfig.MaxPollVotersPerBatch,
                async (info, ct) =>
                {
                    var model = await client.ApiClient.GetPollAnswerVotersAsync(channelId, msgId, answerId, info.PageSize, info.Position, options).ConfigureAwait(false);
                    return model.Users.Select(x => RestUser.Create(client, x)).ToImmutableArray();
                },
                nextPage: (info, lastPage) =>
                {
                    if (lastPage.Count != DiscordConfig.MaxPollVotersPerBatch)
                        return false;

                    info.Position = lastPage.Max(x => x.Id);
                    return true;
                },
                count: limit,
                start: afterId
            );
        }

        public static Task<Message> EndPollAsync(ulong channelId, ulong messageId, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.ExpirePollAsync(channelId, messageId, options);
    }
}
