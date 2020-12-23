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
        public static async Task<Model> ModifyAsync(IMessage msg, BaseDiscordClient client, Action<MessageProperties> func,
            RequestOptions options)
        {
            var args = new MessageProperties();
            func(args);

            if (msg.Author.Id != client.CurrentUser.Id && (args.Content.IsSpecified || args.Embed.IsSpecified || args.AllowedMentions.IsSpecified))
                throw new InvalidOperationException("Only the author of a message may modify the message content, embed, or allowed mentions.");

            bool hasText = args.Content.IsSpecified ? !string.IsNullOrEmpty(args.Content.Value) : !string.IsNullOrEmpty(msg.Content);
            bool hasEmbed = args.Embed.IsSpecified ? args.Embed.Value != null : msg.Embeds.Any();
            if (!hasText && !hasEmbed)
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

            var apiArgs = new API.Rest.ModifyMessageParams
            {
                Content = args.Content,
                Embed = args.Embed.IsSpecified ? args.Embed.Value.ToModel() : Optional.Create<API.Embed>(),
                Flags = args.Flags.IsSpecified ? args.Flags.Value : Optional.Create<MessageFlags?>(),
                AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value.ToModel() : Optional.Create<API.AllowedMentions>(),
            };
            return await client.ApiClient.ModifyMessageAsync(msg.Channel.Id, msg.Id, apiArgs, options).ConfigureAwait(false);
        }

        public static Task DeleteAsync(IMessage msg, BaseDiscordClient client, RequestOptions options)
            => DeleteAsync(msg.Channel.Id, msg.Id, client, options);

        public static async Task DeleteAsync(ulong channelId, ulong msgId, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.DeleteMessageAsync(channelId, msgId, options).ConfigureAwait(false);
        }

        public static async Task SuppressEmbedsAsync(IMessage msg, BaseDiscordClient client, bool suppress, RequestOptions options)
        {
            var apiArgs = new API.Rest.SuppressEmbedParams
            {
                Suppressed = suppress
            };
            await client.ApiClient.SuppressEmbedAsync(msg.Channel.Id, msg.Id, apiArgs, options).ConfigureAwait(false);
        }

        public static async Task AddReactionAsync(IMessage msg, IEmote emote, BaseDiscordClient client, RequestOptions options)
        {
            await client.ApiClient.AddReactionAsync(msg.Channel.Id, msg.Id, emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name), options).ConfigureAwait(false);
        }

        public static async Task RemoveReactionAsync(IMessage msg, ulong userId, IEmote emote, BaseDiscordClient client, RequestOptions options)
        {
            await client.ApiClient.RemoveReactionAsync(msg.Channel.Id, msg.Id, userId, emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name), options).ConfigureAwait(false);
        }

        public static async Task RemoveAllReactionsAsync(IMessage msg, BaseDiscordClient client, RequestOptions options)
        {
            await client.ApiClient.RemoveAllReactionsAsync(msg.Channel.Id, msg.Id, options).ConfigureAwait(false);
        }

        public static async Task RemoveAllReactionsForEmoteAsync(IMessage msg, IEmote emote, BaseDiscordClient client, RequestOptions options)
        {
            await client.ApiClient.RemoveAllReactionsForEmoteAsync(msg.Channel.Id, msg.Id, emote is Emote e ? $"{e.Name}:{e.Id}" : UrlEncode(emote.Name), options).ConfigureAwait(false);
        }

        public static IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IMessage msg, IEmote emote,
            int? limit, BaseDiscordClient client, RequestOptions options)
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

                    var models = await client.ApiClient.GetReactionUsersAsync(msg.Channel.Id, msg.Id, emoji, args, options).ConfigureAwait(false);
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

        public static async Task PinAsync(IMessage msg, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.AddPinAsync(msg.Channel.Id, msg.Id, options).ConfigureAwait(false);
        }

        public static async Task UnpinAsync(IMessage msg, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.RemovePinAsync(msg.Channel.Id, msg.Id, options).ConfigureAwait(false);
        }

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
                if (index == -1) break;
                int endIndex = text.IndexOf('>', index + 1);
                if (endIndex == -1) break;
                if (CheckWrappedCode()) break;
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
                    index = index + 1;
                    continue;
                }
                index = endIndex + 1;
            }

            index = 0;
            codeIndex = 0;
            while (true)
            {
                index = text.IndexOf("@everyone", index);
                if (index == -1) break;
                if (CheckWrappedCode()) break;
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
                if (index == -1) break;
                if (CheckWrappedCode()) break;
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

        public static async Task CrosspostAsync(ulong channelId, ulong msgId, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.CrosspostAsync(channelId, msgId, options).ConfigureAwait(false);
        }

        public static IUser GetAuthor(BaseDiscordClient client, IGuild guild, UserModel model, ulong? webhookId)
        {
            IUser author = null;
            if (guild != null)
                author = guild.GetUserAsync(model.Id, CacheMode.CacheOnly).Result;
            if (author == null)
                author = RestUser.Create(client, guild, model, webhookId);
            return author;
        }
    }
}
