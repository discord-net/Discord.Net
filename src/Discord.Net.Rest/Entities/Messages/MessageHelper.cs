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
        public static async Task<Model> ModifyAsync(IMessage msg, BaseDiscordClient client, Action<ModifyMessageParams> func,
            RequestOptions options)
        {
            var args = new ModifyMessageParams();
            func(args);
            return await client.ApiClient.ModifyMessageAsync(msg.Channel.Id, msg.Id, args, options).ConfigureAwait(false);
        }
        public static async Task DeleteAsync(IMessage msg, BaseDiscordClient client,
            RequestOptions options)
        {
            await client.ApiClient.DeleteMessageAsync(msg.Channel.Id, msg.Id, options).ConfigureAwait(false);
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

        public static ImmutableArray<ITag> ParseTags(string text, IMessageChannel channel, IGuild guild, ImmutableArray<IUser> userMentions)
        {
            var tags = new SortedList<int, ITag>();
            
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
                    tags.Add(index, new Tag<IUser>(TagType.UserMention, index, content.Length, id, mentionedUser));
                }
                else if (MentionUtils.TryParseChannel(content, out id))
                {
                    IChannel mentionedChannel = null;
                    if (guild != null)
                        mentionedChannel = guild.GetChannelAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult();
                    tags.Add(index, new Tag<IChannel>(TagType.ChannelMention, index, content.Length, id, mentionedChannel));
                }
                else if (MentionUtils.TryParseRole(content, out id))
                {
                    IRole mentionedRole = null;
                    if (guild != null)
                        mentionedRole = guild.GetRole(id);
                    tags.Add(index, new Tag<IRole>(TagType.RoleMention, index, content.Length, id, mentionedRole));
                }
                else
                {
                    Emoji emoji;
                    if (Emoji.TryParse(content, out emoji))
                        tags.Add(index, new Tag<Emoji>(TagType.Emoji, index, content.Length, id, emoji));
                }
                index = endIndex + 1;
            }

            index = 0;
            while (true)
            {
                index = text.IndexOf("@everyone", index);
                if (index == -1) break;

                tags.Add(index, new Tag<object>(TagType.EveryoneMention, index, "@everyone".Length, 0, null));
                index++;
            }

            index = 0;
            while (true)
            {
                index = text.IndexOf("@here", index);
                if (index == -1) break;

                tags.Add(index, new Tag<object>(TagType.HereMention, index, "@here".Length, 0, null));
                index++;
            }

            return tags.Values.ToImmutableArray();
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
    }
}
