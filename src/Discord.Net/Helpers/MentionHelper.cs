using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord
{
	internal static class MentionHelper
	{
		private static readonly Regex _userRegex, _channelRegex;

		static MentionHelper()
		{
			_userRegex = new Regex(@"<@(\d+?)>", RegexOptions.Compiled);
			_channelRegex = new Regex(@"<#(\d+?)>", RegexOptions.Compiled);
		}

		public static string ConvertToNames(DiscordClient client, string text)
		{
			text = _userRegex.Replace(text, new MatchEvaluator(e =>
			{
				string id = e.Value.Substring(2, e.Value.Length - 3);
				var user = client.Users[id];
				if (user != null)
					return '@' + user.Name;
				else //User not found
					return e.Value;
			}));
			text = _channelRegex.Replace(text, new MatchEvaluator(e =>
			{
				string id = e.Value.Substring(2, e.Value.Length - 3);
				var channel = client.Channels[id];
				if (channel != null)
					return channel.Name;
				else //Channel not found
					return e.Value;
			}));
			return text;
		}

		public static IEnumerable<string> GetUserIds(string text)
		{
			return _userRegex.Matches(text)
				.OfType<Match>()
				.Select(x => x.Groups[1].Value)
				.Where(x => x != null);
		}
	}
}
