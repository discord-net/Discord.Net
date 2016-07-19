using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord
{
    public static class MentionUtils
    {
        private static readonly Regex _userRegex = new Regex(@"<@!?([0-9]+)>", RegexOptions.Compiled);
        private static readonly Regex _channelRegex = new Regex(@"<#([0-9]+)>", RegexOptions.Compiled);
        private static readonly Regex _roleRegex = new Regex(@"<@&([0-9]+)>", RegexOptions.Compiled);

        //Unsure the system can be positive a user doesn't have a nickname, assume useNickname = true (source: Jake)
        internal static string Mention(IUser user, bool useNickname = true) => useNickname ? $"<@!{user.Id}>" : $"<@{user.Id}>";
        internal static string Mention(IChannel channel) => $"<#{channel.Id}>";
        internal static string Mention(IRole role) => $"<&{role.Id}>";

        /// <summary> Parses a provided user mention string. </summary>
        public static ulong ParseUser(string mentionText)
        {
            ulong id;
            if (TryParseUser(mentionText, out id))
                return id;
            throw new ArgumentException("Invalid mention format", nameof(mentionText));
        }
        /// <summary> Tries to parse a provided user mention string. </summary>
        public static bool TryParseUser(string mentionText, out ulong userId)
        {
            mentionText = mentionText.Trim();
            if (mentionText.Length >= 3 && mentionText[0] == '<' && mentionText[1] == '@' && mentionText[mentionText.Length - 1] == '>')
            {
                if (mentionText.Length >= 4 && mentionText[2] == '!')
                    mentionText = mentionText.Substring(3, mentionText.Length - 4); //<@!123>
                else
                    mentionText = mentionText.Substring(2, mentionText.Length - 3); //<@123>
                
                if (ulong.TryParse(mentionText, NumberStyles.None, CultureInfo.InvariantCulture, out userId))
                    return true;
            }
            userId = 0;
            return false;
        }

        /// <summary> Parses a provided channel mention string. </summary>
        public static ulong ParseChannel(string mentionText)
        {
            ulong id;
            if (TryParseChannel(mentionText, out id))
                return id;
            throw new ArgumentException("Invalid mention format", nameof(mentionText));
        }
        /// <summary>Tries to parse a provided channel mention string. </summary>
        public static bool TryParseChannel(string mentionText, out ulong channelId)
        {
            mentionText = mentionText.Trim();
            if (mentionText.Length >= 3 && mentionText[0] == '<' && mentionText[1] == '#' && mentionText[mentionText.Length - 1] == '>')
            {
                mentionText = mentionText.Substring(2, mentionText.Length - 3); //<#123>
                
                if (ulong.TryParse(mentionText, NumberStyles.None, CultureInfo.InvariantCulture, out channelId))
                    return true;
            }
            channelId = 0;
            return false;
        }

        /// <summary> Parses a provided role mention string. </summary>
        public static ulong ParseRole(string mentionText)
        {
            ulong id;
            if (TryParseRole(mentionText, out id))
                return id;
            throw new ArgumentException("Invalid mention format", nameof(mentionText));
        }
        /// <summary>Tries to parse a provided role mention string. </summary>
        public static bool TryParseRole(string mentionText, out ulong roleId)
        {
            mentionText = mentionText.Trim();
            if (mentionText.Length >= 4 && mentionText[0] == '<' && mentionText[1] == '@' && mentionText[2] == '&' && mentionText[mentionText.Length - 1] == '>')
            {
                mentionText = mentionText.Substring(3, mentionText.Length - 4); //<@&123>
                
                if (ulong.TryParse(mentionText, NumberStyles.None, CultureInfo.InvariantCulture, out roleId))
                    return true;
            }
            roleId = 0;
            return false;
        }
        
        internal static ImmutableArray<IUser> GetUserMentions(string text, IMessageChannel channel, IReadOnlyCollection<IUser> mentionedUsers)
        {
            var matches = _userRegex.Matches(text);
            var builder = ImmutableArray.CreateBuilder<IUser>(matches.Count);
            foreach (var match in matches.OfType<Match>())
            {
                ulong id;
                if (ulong.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    IUser user = null;

                    //Verify this user was actually mentioned
                    foreach (var userMention in mentionedUsers)
                    {
                        if (userMention.Id == id)
                        {
                            if (channel.IsAttached) //Waiting this sync is safe because it's using a cache
                                user = channel.GetUserAsync(id).GetAwaiter().GetResult() as IUser;
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
                {
                    /*var channel = guild.GetChannelAsync(id).GetAwaiter().GetResult();
                    if (channel != null)
                        builder.Add(channel);*/
                    builder.Add(id);
                }
            }
            return builder.ToImmutable();
        }
        internal static ImmutableArray<IRole> GetRoleMentions(string text, IGuild guild)
        {
            var matches = _roleRegex.Matches(text);
            var builder = ImmutableArray.CreateBuilder<IRole>(matches.Count);
            foreach (var match in matches.OfType<Match>())
            {
                ulong id;
                if (ulong.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    var role = guild.GetRole(id);
                    if (role != null)
                        builder.Add(role);
                }
            }
            return builder.ToImmutable();
        }
        
        internal static string ResolveUserMentions(string text, IMessageChannel channel, IReadOnlyCollection<IUser> mentions, UserResolveMode mode)
        {
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
                            case UserResolveMode.NameOnly:
                            default:
                                return $"@{name}";
                            case UserResolveMode.NameAndDiscriminator:
                                return $"@{name}#{user.Discriminator}";
                        }
                    }
                }
                return e.Value;
            }));
        }
        internal static string ResolveChannelMentions(string text, IGuild guild)
        {
            if (guild.IsAttached) //It's too expensive to do a channel lookup in REST mode
            {
                return _channelRegex.Replace(text, new MatchEvaluator(e =>
                {
                    ulong id;
                    if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    {
                        IGuildChannel channel = null;
                        channel = guild.GetChannelAsync(id).GetAwaiter().GetResult();
                        if (channel != null)
                            return '#' + channel.Name;
                    }
                    return e.Value;
                }));
            }
            return text;
        }
        internal static string ResolveRoleMentions(string text, IGuild guild, IReadOnlyCollection<IRole> mentions)
        {
            return _roleRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
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
                        return '@' + role.Name;
                }
                return e.Value;
            }));
        }
    }
}
