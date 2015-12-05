using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Discord
{
    public static class Mention
	{
		private static readonly Regex _userRegex = new Regex(@"<@([0-9]+?)>", RegexOptions.Compiled);
		private static readonly Regex _channelRegex = new Regex(@"<#([0-9]+?)>", RegexOptions.Compiled);
		private static readonly Regex _roleRegex = new Regex(@"@everyone", RegexOptions.Compiled);
		
		/// <summary> Returns the string used to create a user mention. </summary>
		[Obsolete("Use User.Mention instead")]
		public static string User(User user)
			=> $"<@{user.Id}>";
		/// <summary> Returns the string used to create a user mention. </summary>
		[Obsolete("Use GlobalUser.Mention instead")]
		public static string User(GlobalUser user)
			=> $"<@{user.Id}>";
		/// <summary> Returns the string used to create a channel mention. </summary>
		[Obsolete("Use Channel.Mention instead")]
		public static string Channel(Channel channel)
			=> $"<#{channel.Id}>";
		/// <summary> Returns the string used to create a mention to everyone in a channel. </summary>
		[Obsolete("Use Role.Mention instead")]
		public static string Everyone()
			=> $"@everyone";

		internal static string CleanUserMentions(DiscordClient client, Server server, string text, List<User> users = null)
		{
			return _userRegex.Replace(text, new MatchEvaluator(e =>
			{
				long id = IdConvert.ToLong(e.Value.Substring(2, e.Value.Length - 3));
				var user = client.Users[id, server?.Id];
				if (user != null)
				{
					if (users != null)
						users.Add(user);
					return '@' + user.Name;
				}
				else //User not found
					return '@' + e.Value;
			}));
		}
		internal static string CleanChannelMentions(DiscordClient client, Server server, string text, List<Channel> channels = null)
		{
			return _channelRegex.Replace(text, new MatchEvaluator(e =>
			{
				long id = IdConvert.ToLong(e.Value.Substring(2, e.Value.Length - 3));
				var channel = client.Channels[id];
				if (channel != null && channel.Server.Id == server.Id)
				{
					if (channels != null)
						channels.Add(channel);
					return '#' + channel.Name;
				}
				else //Channel not found
					return '#' + e.Value;
			}));
		}
		/*internal static string CleanRoleMentions(DiscordClient client, User user, Channel channel, string text, List<Role> roles = null)
		{
			return _roleRegex.Replace(text, new MatchEvaluator(e =>
			{
				if (roles != null && user.GetPermissions(channel).MentionEveryone)
					roles.Add(channel.Server.EveryoneRole);
				return e.Value;
			}));
		}*/

		/// <summary>Resolves all mentions in a provided string to those users, channels or roles' names.</summary>
		public static string Resolve(Message source, string text)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (text == null) throw new ArgumentNullException(nameof(text));

			return Resolve(source.Server, text);
        }

		/// <summary>Resolves all mentions in a provided string to those users, channels or roles' names.</summary>
		public static string Resolve(Server server, string text)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			var client = server?.Client;
            text = CleanUserMentions(client, server, text);
			if (server != null)
			{
				text = CleanChannelMentions(client, server, text);
				//text = CleanRoleMentions(_client, User, channel, text);
			}
			return text;
		}
    }
}
