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

        internal static string Mention(IUser user, bool useNickname) => useNickname ? $"<@!{user.Id}>" : $"<@{user.Id}>";
        internal static string Mention(IChannel channel) => $"<#{channel.Id}>";
        internal static string Mention(IRole role) => $"<&{role.Id}>";

        /// <summary> Parses a provided user mention string. </summary>
        public static ulong ParseUser(string mentionText)
        {
            mentionText = mentionText.Trim();
            if (mentionText.Length >= 3 && mentionText[0] == '<' && mentionText[1] == '@' && mentionText[mentionText.Length - 1] == '>')
            {
                if (mentionText.Length >= 4 && mentionText[2] == '!')
                    mentionText = mentionText.Substring(3, mentionText.Length - 4); //<@!123>
                else
                    mentionText = mentionText.Substring(2, mentionText.Length - 3); //<@123>

                ulong id;
                if (ulong.TryParse(mentionText, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    return id;
            }
            throw new ArgumentException("Invalid mention format", nameof(mentionText));
        }
        /// <summary> Parses a provided channel mention string. </summary>
        public static ulong ParseChannel(string mentionText)
        {
            mentionText = mentionText.Trim();
            if (mentionText.Length >= 3 && mentionText[0] == '<' && mentionText[1] == '#' && mentionText[mentionText.Length - 1] == '>')
            {
                mentionText = mentionText.Substring(2, mentionText.Length - 3); //<#123>

                ulong id;
                if (ulong.TryParse(mentionText, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    return id;
            }
            throw new ArgumentException("Invalid mention format", nameof(mentionText));
        }
        /// <summary> Parses a provided role mention string. </summary>
        public static ulong ParseRole(string mentionText)
        {
            mentionText = mentionText.Trim();
            if (mentionText.Length >= 4 && mentionText[0] == '<' && mentionText[1] == '@' && mentionText[2] == '&' && mentionText[mentionText.Length - 1] == '>')
            {
                mentionText = mentionText.Substring(3, mentionText.Length - 4); //<@&123>

                ulong id;
                if (ulong.TryParse(mentionText, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    return id;
            }
            throw new ArgumentException("Invalid mention format", nameof(mentionText));
        }

        /// <summary> Gets the ids of all users mentioned in a provided text.</summary>
        public static IReadOnlyList<ulong> GetUserMentions(string text) => GetMentions(text, _userRegex).ToArray();
        /// <summary> Gets the ids of all channels mentioned in a provided text.</summary>
        public static IReadOnlyList<ulong> GetChannelMentions(string text) => GetMentions(text, _channelRegex).ToArray();
        /// <summary> Gets the ids of all roles mentioned in a provided text.</summary>
        public static IReadOnlyList<ulong> GetRoleMentions(string text) => GetMentions(text, _roleRegex).ToArray();
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

        internal static string CleanUserMentions(string text, API.User[] mentions)
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
        }
        internal static string CleanUserMentions<T>(string text, IReadOnlyDictionary<ulong, T> users, ImmutableArray<T>.Builder mentions = null)
            where T : IGuildUser
        {
            return _channelRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    T user;
                    if (users.TryGetValue(id, out user))
                    {
                        if (users != null)
                            mentions.Add(user);
                        if (e.Value[2] == '!' && user.Nickname != null)
                            return '@' + user.Nickname;
                        else
                            return '@' + user.Username;
                    }
                }
                return e.Value;
            }));
        }
        internal static string CleanChannelMentions<T>(string text, IReadOnlyDictionary<ulong, T> channels, ImmutableArray<T>.Builder mentions = null)
            where T : IGuildChannel
        {
            return _channelRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    T channel;
                    if (channels.TryGetValue(id, out channel))
                    {
                        if (channels != null)
                            mentions.Add(channel);
                        return '#' + channel.Name;
                    }
                }
                return e.Value;
            }));
        }
        internal static string CleanRoleMentions<T>(string text, IReadOnlyDictionary<ulong, T> roles, ImmutableArray<T>.Builder mentions = null)
            where T : IRole
        {
            return _channelRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    T role;
                    if (roles.TryGetValue(id, out role))
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
