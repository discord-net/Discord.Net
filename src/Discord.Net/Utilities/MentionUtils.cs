using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord
{
    public static class MentionUtils
    {
        private static readonly Regex _userRegex = new Regex(@"<@!?([0-9]+)>", RegexOptions.Compiled);
        private static readonly Regex _channelRegex = new Regex(@"<#([0-9]+)>", RegexOptions.Compiled);
        private static readonly Regex _roleRegex = new Regex(@"<@&([0-9]+)>", RegexOptions.Compiled);

        internal static string Mention(IUser user, bool useNickname) => useNickname ? $"<@!{user.Id}>" : $"<@{user.Id}>";
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

        /// <summary> Gets the ids of all users mentioned in a provided text.</summary>
        public static ImmutableArray<ulong> GetUserMentions(string text) => GetMentions(text, _userRegex).ToImmutable();
        /// <summary> Gets the ids of all channels mentioned in a provided text.</summary>
        public static ImmutableArray<ulong> GetChannelMentions(string text) => GetMentions(text, _channelRegex).ToImmutable();
        /// <summary> Gets the ids of all roles mentioned in a provided text.</summary>
        public static ImmutableArray<ulong> GetRoleMentions(string text) => GetMentions(text, _roleRegex).ToImmutable();
        private static ImmutableArray<ulong>.Builder GetMentions(string text, Regex regex)
        {
            var matches = regex.Matches(text);
            var builder = ImmutableArray.CreateBuilder<ulong>(matches.Count);
            foreach (var match in matches.OfType<Match>())
            {
                ulong id;
                if (ulong.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    builder.Add(id);
            }
            return builder;
        }

        /*internal static string CleanUserMentions(string text, ImmutableArray<User> mentions)
        {
            return _userRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    for (int i = 0; i < mentions.Length; i++)
                    {
                        var mention = mentions[i];
                        if (mention.Id == id)
                            return '@' + mention.Username;
                    }
                }
                return e.Value;
            }));
        }*/
        internal static string CleanUserMentions(string text, IMessageChannel channel, IReadOnlyCollection<IUser> fallbackUsers, ImmutableArray<IUser>.Builder mentions = null)
        {
            return _userRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    IUser user = null;
                    if (channel != null)
                        user = channel.GetUserAsync(id).GetAwaiter().GetResult() as IUser;
                    if (user == null)
                    {
                        foreach (var fallbackUser in fallbackUsers)
                        {
                            if (fallbackUser.Id == id)
                            {
                                user = fallbackUser;
                                break;
                            }
                        }
                    }
                    if (user != null)
                    {
                        mentions.Add(user);

                        if (e.Value[2] == '!')
                        {
                            var guildUser = user as IGuildUser;
                            if (guildUser != null && guildUser.Nickname != null)
                                return '@' + guildUser.Nickname;
                        }
                        return '@' + user.Username;
                    }
                }
                return e.Value;
            }));
        }
        internal static string CleanChannelMentions(string text, IGuild guild, ImmutableArray<ulong>.Builder mentions = null)
        {
            return _channelRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    var channel = guild.GetChannelAsync(id).GetAwaiter().GetResult() as IGuildChannel;
                    if (channel != null)
                    {
                        if (mentions != null)
                            mentions.Add(channel.Id);
                        return '#' + channel.Name;
                    }
                }
                return e.Value;
            }));
        }
        internal static string CleanRoleMentions(string text, IGuild guild, ImmutableArray<IRole>.Builder mentions = null)
        {
            return _roleRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    var role = guild.GetRole(id);
                    if (role != null)
                    {
                        if (mentions != null)
                            mentions.Add(role);
                        return '@' + role.Name;
                    }
                }
                return e.Value;
            }));
        }
    }
}
