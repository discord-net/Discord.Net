using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    internal static class MessageHelper
    {
        public static async Task<Model> ModifyAsync(IMessage msg, BaseDiscordClient client, Action<MessageProperties> func,
            RequestOptions options)
        {
            var args = new MessageProperties();
            func(args);
            var apiArgs = new API.Rest.ModifyMessageParams
            {
                Content = args.Content,
                Embed = args.Embed.IsSpecified ? args.Embed.Value.ToModel() : Optional.Create<API.Embed>()
            };
            return await client.ApiClient.ModifyMessageAsync(msg.Channel.Id, msg.Id, apiArgs, options).ConfigureAwait(false);
        }
        public static async Task DeleteAsync(IMessage msg, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.DeleteMessageAsync(msg.Channel.Id, msg.Id, options).ConfigureAwait(false);
        }

        public static async Task AddReactionAsync(IMessage msg, Emoji emoji, BaseDiscordClient client, RequestOptions options)
            => await AddReactionAsync(msg, $"{emoji.Name}:{emoji.Id}", client, options).ConfigureAwait(false);
        public static async Task AddReactionAsync(IMessage msg, string emoji, BaseDiscordClient client, RequestOptions options)
        {
            await client.ApiClient.AddReactionAsync(msg.Channel.Id, msg.Id, emoji, options).ConfigureAwait(false);
        }

        public static async Task RemoveReactionAsync(IMessage msg, IUser user, Emoji emoji, BaseDiscordClient client, RequestOptions options)
            => await RemoveReactionAsync(msg, user, emoji.Id == null ? emoji.Name : $"{emoji.Name}:{emoji.Id}", client, options).ConfigureAwait(false);
        public static async Task RemoveReactionAsync(IMessage msg, IUser user, string emoji, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.RemoveReactionAsync(msg.Channel.Id, msg.Id, user.Id, emoji, options).ConfigureAwait(false);
        }

        public static async Task RemoveAllReactionsAsync(IMessage msg, BaseDiscordClient client, RequestOptions options)
        {
            await client.ApiClient.RemoveAllReactionsAsync(msg.Channel.Id, msg.Id, options);
        }

        public static async Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IMessage msg, string emoji,
            Action<GetReactionUsersParams> func, BaseDiscordClient client, RequestOptions options)
        {
            var args = new GetReactionUsersParams();
            func(args);
            return (await client.ApiClient.GetReactionUsersAsync(msg.Channel.Id, msg.Id, emoji, args, options).ConfigureAwait(false)).Select(u => RestUser.Create(client, u)).ToImmutableArray();
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
            while (true)
            {
                index = text.IndexOf('<', index);
                if (index == -1) break;
                int endIndex = text.IndexOf('>', index + 1);
                if (endIndex == -1) break;
                string content = text.Substring(index, endIndex - index + 1);

                ulong id;
                if (MentionUtils.TryParseUser(content, out id))
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
                else if (Emoji.TryParse(content, out var emoji))
                    tags.Add(new Tag<Emoji>(TagType.Emoji, index, content.Length, emoji.Id ?? 0, emoji));
                else //Bad Tag
                {
                    index = index + 1;
                    continue;
                }
                index = endIndex + 1;
            }

            index = 0;
            while (true)
            {
                index = text.IndexOf("@everyone", index);
                if (index == -1) break;

                var tagIndex = FindIndex(tags, index);
                if (tagIndex.HasValue)
                    tags.Insert(tagIndex.Value, new Tag<object>(TagType.EveryoneMention, index, "@everyone".Length, 0, null));
                index++;
            }

            index = 0;
            while (true)
            {
                index = text.IndexOf("@here", index);
                if (index == -1) break;

                var tagIndex = FindIndex(tags, index);
                if (tagIndex.HasValue)
                    tags.Insert(tagIndex.Value, new Tag<object>(TagType.HereMention, index, "@here".Length, 0, null));
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
            if (msg.Type != MessageType.Default)
                return MessageSource.System;
            else if (msg.WebhookId.IsSpecified)
                return MessageSource.Webhook;
            else if (msg.Author.GetValueOrDefault()?.Bot.GetValueOrDefault(false) == true)
                return MessageSource.Bot;
            return MessageSource.User;
        }
    }
}
