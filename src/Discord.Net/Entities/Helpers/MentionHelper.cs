using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Discord
{
    internal static class MentionHelper
    {
        private static readonly Regex _userRegex = new Regex(@"<@([0-9]+)>");
        private static readonly Regex _channelRegex = new Regex(@"<#([0-9]+)>");
        private static readonly Regex _roleRegex = new Regex(@"@everyone");

        internal static string Mention(IUser user) => $"<@{user.Id}>";
        internal static string Mention(IChannel channel) => $"<#{channel.Id}>";
        internal static string Mention(Role role) => role.IsEveryone ? "@everyone" : "";

        internal static string CleanUserMentions(GuildChannel channel, string text, ImmutableArray<GuildUser>.Builder users = null)
        {
            var guild = channel.Guild;
            return _userRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    var user = guild.GetUser(id); //We're able to mention users outside of our channel
                    if (user != null)
                    {
                        if (users != null)
                            users.Add(user);
                        return '@' + user.Username;
                    }
                }
                return e.Value; //User not found or parse failed
            }));
        }
        internal static string CleanChannelMentions(GuildChannel channel, string text, ImmutableArray<GuildChannel>.Builder channels = null)
        {
            var guild = channel.Guild;
            return _channelRegex.Replace(text, new MatchEvaluator(e =>
            {
                ulong id;
                if (ulong.TryParse(e.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    var mentionedChannel = guild.GetChannel(id);
                    if (mentionedChannel != null && mentionedChannel.Guild.Id == guild.Id)
                    {
                        if (channels != null)
                            channels.Add(mentionedChannel);
                        return '#' + mentionedChannel.Name;
                    }
                }
                return e.Value; //Channel not found or parse failed
            }));
        }
        /*internal static string CleanRoleMentions(User user, IPublicChannel channel, string text, ImmutableArray<Role>.Builder roles = null)
        {
            var guild = channel.Guild;
            if (guild == null) return text;

            return _roleRegex.Replace(text, new MatchEvaluator(e =>
            {
                if (roles != null && user.GetPermissions(channel).MentionEveryone)
                    roles.Add(guild.EveryoneRole);
                return e.Value;
            }));
        }*/

        internal static ulong GetUserId(string mention)
        {
            mention = mention.Trim();
            if (mention.Length >= 3 && mention[0] == '<' && mention[1] == '@' && mention[mention.Length - 1] == '>')
            {
                mention = mention.Substring(2, mention.Length - 3);

                ulong id;
                if (ulong.TryParse(mention, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    return id;
            }
            throw new ArgumentException("Invalid mention format", nameof(mention));
        }
        internal static ulong GetChannelId(string mention)
        {
            mention = mention.Trim();
            if (mention.Length >= 3 && mention[0] == '<' && mention[1] == '#' && mention[mention.Length - 1] == '>')
            {
                mention = mention.Substring(2, mention.Length - 3);

                ulong id;
                if (ulong.TryParse(mention, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    return id;
            }
            throw new ArgumentException("Invalid mention format", nameof(mention));
        }

        internal static string ResolveMentions(IChannel channel, string text)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (text == null) throw new ArgumentNullException(nameof(text));

            var publicChannel = channel as GuildChannel;
            if (publicChannel != null)
            {
                text = CleanUserMentions(publicChannel, text);
                text = CleanChannelMentions(publicChannel, text);
                //text = CleanRoleMentions(publicChannel, text);
            }
            return text;
        }
    }
}
