using System;
using System.Threading.Tasks;

namespace Discord
{
	public sealed class CommandBuilder
	{
		private readonly DiscordBotClient _client;
		private readonly string _prefix;
		private readonly bool _useWhitelist;

		public CommandBuilder(DiscordBotClient client, string prefix, bool useWhitelist = false)
		{
			_client = client;
			_prefix = prefix;
			_useWhitelist = useWhitelist;
        }

		public void AddCommandGroup(string cmd, Action<CommandBuilder> config, bool useWhitelist = false)
		{
			config(new CommandBuilder(_client, _prefix + ' ' + cmd, useWhitelist));
		}
		public void AddCommand(string cmd, int minArgs, int maxArgs, Action<DiscordBotClient.CommandEventArgs> handler, bool? useWhitelist = null)
		{
			AddCommand(cmd, minArgs, maxArgs, e => { handler(e); return null; }, useWhitelist);
		}
		public void AddCommand(string cmd, int minArgs, int maxArgs, Func<DiscordBotClient.CommandEventArgs, Task> handler, bool? useWhitelist = null)
		{
			_client.AddCommand(cmd != "" ? _prefix + ' ' + cmd : _prefix, minArgs, maxArgs, handler, useWhitelist ?? _useWhitelist);
		}
	}
}
