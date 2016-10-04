using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord
{
    internal static class MentionsHelper
    {
        private static readonly Regex _userRegex = new Regex(@"<@!?([0-9]+)>", RegexOptions.Compiled);
        private static readonly Regex _channelRegex = new Regex(@"<#([0-9]+)>", RegexOptions.Compiled);
        private static readonly Regex _roleRegex = new Regex(@"<@&([0-9]+)>", RegexOptions.Compiled);

        //If the system can't be positive a user doesn't have a nickname, assume useNickname = true (source: Jake)
        internal static string MentionUser(ulong id, bool useNickname = true) => useNickname ? $"<@!{id}>" : $"<@{id}>";
        internal static string MentionChannel(ulong id) => $"<#{id}>";
        internal static string MentionRole(ulong id) => $"<@&{id}>";

        internal static ImmutableArray<TUser> GetUserMentions<TUser>(string text, IMessageChannel channel, IReadOnlyCollection<TUser> mentionedUsers)
            where TUser : class, IUser
        {
            var matches = _userRegex.Matches(text);
            var builder = ImmutableArray.CreateBuilder<TUser>(matches.Count);
            foreach (var match in matches.OfType<Match>())
            {
                ulong id;
                if (ulong.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    TUser user = null;

                    //Verify this user was actually mentioned
                    foreach (var userMention in mentionedUsers)
                    {
                        if (userMention.Id == id)
                        {
                            user = channel?.GetUserAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult() as TUser;
                            if (user == null) //User not found, fallback to basic mention info
                                user = userMention;
                            break;
                        }
                    }

                    if (user != null)
                        builder.Add(user);
                }
            }
            return builder.ToImmutable();
        }
        internal static ImmutableArray<ulong> GetChannelMentions(string text, IGuild guild)
        {
            var matches = _channelRegex.Matches(text);
            var builder = ImmutableArray.CreateBuilder<ulong>(matches.Count);
            foreach (var match in matches.OfType<Match>())
            {
                ulong id;
                if (ulong.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    builder.Add(id);
            }
            return builder.ToImmutable();
        }
        internal static ImmutableArray<TRole> GetRoleMentions<TRole>(string text, IGuild guild)
            where TRole : class, IRole
        {
            var matches = _roleRegex.Matches(text);
            var builder = ImmutableArray.CreateBuilder<TRole>(matches.Count);
            foreach (var match in matches.OfType<Match>())
            {
                ulong id;
                if (ulong.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    var role = guild.GetRole(id) as TRole;
                    if (role != null)
                        builder.Add(role);
                }
            }
            return builder.ToImmutable();
        }

        internal static string ResolveUserMentions(string text, IMessageChannel channel, IReadOnlyCollection<IUser> mentions, UserMentionHandling mode)
        {
            if (mode == UserMentionHandling.Ignore) return text;

            return _userRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    IUser user = null;
                    foreach (var mention in mentions)
                    {
                        if (mention.Id == id)
                        {
                            user = mention;
                            break;
                        }
                    }
                    if (user != null)
                    {
                        string name = user.Username;

                        var guildUser = user as IGuildUser;
                        if (e.Value[2] == '!')
                        {
                            if (guildUser != null && guildUser.Nickname != null)
                                name = guildUser.Nickname;
                        }

                        switch (mode)
                        {
                            case UserMentionHandling.Remove:
                            default:
                                return "";
                            case UserMentionHandling.Name:
                                return $"@{name}";
                            case UserMentionHandling.NameAndDiscriminator:
                                return $"@{name}#{user.Discriminator}";
                        }
                    }
                }
                return e.Value;
            }));
        }
        internal static string ResolveChannelMentions(string text, IGuild guild, ChannelMentionHandling mode)
        {
            if (mode == ChannelMentionHandling.Ignore) return text;

            return _channelRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    switch (mode)
                    {
                        case ChannelMentionHandling.Remove:
                            return "";
                        case ChannelMentionHandling.Name:
                            IGuildChannel channel = null;
                            channel = guild.GetChannelAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult();
                            if (channel != null)
                                return $"#{channel.Name}";
                            else
                                return $"#deleted-channel";
                    }
                }
                return e.Value;
            }));
        }
        internal static string ResolveRoleMentions(string text, IReadOnlyCollection<IRole> mentions, RoleMentionHandling mode)
        {
            if (mode == RoleMentionHandling.Ignore) return text;

            return _roleRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    switch (mode)
                    {
                        case RoleMentionHandling.Remove:
                            return "";
                        case RoleMentionHandling.Name:
                            IRole role = null;
                            foreach (var mention in mentions)
                            {
                                if (mention.Id == id)
                                {
                                    role = mention;
                                    break;
                                }
                            }
                            if (role != null)
                                return $"@{role.Name}";
                            else
                                return $"@deleted-role";
                    }
                }
                return e.Value;
            }));
        }
        internal static string ResolveEveryoneMentions(string text, EveryoneMentionHandling mode)
        {
            if (mode == EveryoneMentionHandling.Ignore) return text;

            switch (mode)
            {
                case EveryoneMentionHandling.Sanitize:
                    return text.Replace("@everyone", "@\x200beveryone").Replace("@here", "@\x200bhere");
                case EveryoneMentionHandling.Remove:
                default:
                    return text.Replace("@everyone", "").Replace("@here", "");
            }
        }
    }
}
