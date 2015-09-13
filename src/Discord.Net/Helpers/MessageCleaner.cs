using System.Text.RegularExpressions;

namespace Discord.Helpers
{
	//TODO: Better name please?
	internal class MessageCleaner
	{
		private readonly Regex _userRegex, _channelRegex;
		private readonly MatchEvaluator _userRegexEvaluator, _channelRegexEvaluator;

		public MessageCleaner(DiscordClient client)
		{
			_userRegex = new Regex(@"<@\d+?>", RegexOptions.Compiled);
			_userRegexEvaluator = new MatchEvaluator(e =>
				{
					string id = e.Value.Substring(2, e.Value.Length - 3);
					var user = client.Users[id];
					if (user != null)
						return '@' + user.Name;
					else //User not found
					return e.Value;
				});

			_channelRegex = new Regex(@"<#\d+?>", RegexOptions.Compiled);
			_channelRegexEvaluator = new MatchEvaluator(e =>
			{
				string id = e.Value.Substring(2, e.Value.Length - 3);
				var channel = client.Channels[id];
				if (channel != null)
					return channel.Name;
				else //Channel not found
					return e.Value;
			});
		}

		public string Clean(string text)
		{
			text = _userRegex.Replace(text, _userRegexEvaluator);
			text = _channelRegex.Replace(text, _channelRegexEvaluator);
			return text;
		}
	}
}
