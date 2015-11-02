using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord
{
    public static class Mention
	{
		private static readonly Regex _userRegex = new Regex(@"<@([0-9]+?)>", RegexOptions.Compiled);
		private static readonly Regex _channelRegex = new Regex(@"<#([0-9]+?)>", RegexOptions.Compiled);

		/// <summary> Returns the string used to create a user mention. </summary>
		public static string User(User user)
			=> $"<@{user.Id}>";
		/// <summary> Returns the string used to create a channel mention. </summary>
		public static string Channel(Channel channel)
			=> $"<#{channel.Id}>";
		/// <summary> Returns the string used to create a channel mention. </summary>
		public static string Everyone()
			=> $"@everyone";

		internal static string ConvertToNames(DiscordClient client, Server server, string text)
		{
			text = _userRegex.Replace(text, new MatchEvaluator(e =>
			{
				string id = e.Value.Substring(2, e.Value.Length - 3);
				var user = client.Users[id, server?.Id];
				if (user != null)
					return '@' + user.Name;
				else //User not found
					return '@' + e.Value;
			}));
			if (server != null)
			{
				text = _channelRegex.Replace(text, new MatchEvaluator(e =>
				{
					string id = e.Value.Substring(2, e.Value.Length - 3);
					var channel = client.Channels[id];
					if (channel != null && channel.Server.Id == server.Id)
						return '#' + channel.Name;
					else //Channel not found
					return '#' + e.Value;
				}));
			}
			return text;
		}

		internal static IEnumerable<string> GetUserIds(string text)
		{
			return _userRegex.Matches(text)
				.OfType<Match>()
				.Select(x => x.Groups[1].Value)
				.Where(x => x != null);
		}
	}
}
